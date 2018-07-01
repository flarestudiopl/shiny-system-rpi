using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public class SaveZoneCommand
    {
        public int? ZoneId { get; set; }
        public string Name { get; set; }
        public int? TemperatureSensorId { get; set; }
        public ICollection<int> HeaterIds { get; set; }
    }

    public class SaveZoneCommandExecutor : ICommandExecutor<SaveZoneCommand>
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public SaveZoneCommandExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public CommandResult Execute(SaveZoneCommand command, CommandContext context)
        {
            var validationResult = Validate(command, context.ControllerState);

            if (validationResult != null)
            {
                return validationResult;
            }

            Zone existingZone = null;

            if (command.ZoneId.HasValue)
            {
                var existingZoneId = command.ZoneId.Value;
                var zoneState = context.ControllerState.ZoneIdToState[existingZoneId];
                existingZone = zoneState.Zone;

                context.ControllerState.ZoneIdToState.Remove(existingZoneId);

                foreach (var heaterToDisable in zoneState.Zone
                                                         .HeaterIds
                                                         .Except(command.HeaterIds))
                {
                    context.ControllerState.HeaterIdToState[heaterToDisable].OutputState = false;
                }

                context.ControllerState.Model.Zones.Remove(x => x.ZoneId == existingZoneId);
            }

            var zone = BuildNewZone(command, existingZone, context.ControllerState);

            context.ControllerState.Model.Zones.Add(zone);

            _buildingModelSaver.Save(context.ControllerState.Model);

            context.ControllerState.ZoneIdToState.Add(zone.ZoneId,
                                                      new ZoneState
                                                      {
                                                          Zone = zone,
                                                          ControlMode = ZoneControlMode.LowOrDisabled,
                                                          EnableOutputs = false,
                                                          ScheduleState = new ScheduleState
                                                                          {
                                                                              DesiredTemperature = 0.0f,
                                                                              HeatingEnabled = false
                                                                          }
                                                      });

            return CommandResult.Empty;
        }

        private static CommandResult Validate(SaveZoneCommand command, ControllerState controllerState)
        {
            if (command.Name.IsNullOrEmpty())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.NameCantBeEmpty);
            }

            if (command.ZoneId.HasValue && !controllerState.ZoneIdToState.ContainsKey(command.ZoneId.Value))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownZoneId.FormatWith(command.ZoneId.Value));
            }

            foreach (var heaterId in command.HeaterIds)
            {
                if (!controllerState.HeaterIdToState.ContainsKey(heaterId))
                {
                    return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownHeaterId.FormatWith(heaterId));
                }

                if (controllerState.ZoneIdToState
                                   .Select(z => z.Value.Zone)
                                   .Where(x => !command.ZoneId.HasValue || command.ZoneId.Value != x.ZoneId)
                                   .Any(z => z.HeaterIds.Contains(heaterId)))
                {
                    return CommandResult.WithValidationError(Localization.ValidationMessage.HeaterAlreadyInUseByAnotherZone.FormatWith(heaterId));
                }
            }

            return null;
        }

        private static Zone BuildNewZone(SaveZoneCommand command, Zone existingZone, ControllerState controllerState)
        {
            var zone = new Zone
                       {
                           Name = command.Name,
                           HeaterIds = command.HeaterIds.ToHashSet()
                       };

            if (command.TemperatureSensorId.HasValue)
            {
                if (existingZone?.TemperatureControlledZone != null)
                {
                    zone.TemperatureControlledZone = existingZone.TemperatureControlledZone;
                }
                else
                {
                    zone.TemperatureControlledZone = new TemperatureControlledZone
                                                     {
                                                         LowSetPoint = 4.0f,
                                                         HighSetPoint = 20.0f,
                                                         Hysteresis = 0.5f,
                                                         ScheduleDefaultSetPoint = 4.0f
                                                     };
                }

                zone.TemperatureControlledZone.TemperatureSensorId = command.TemperatureSensorId.Value;
            }

            if (existingZone != null)
            {
                zone.Schedule = existingZone.Schedule;
                zone.ZoneId = existingZone.ZoneId;
            }
            else
            {
                zone.ZoneId = (controllerState.ZoneIdToState.Keys.Any() ? controllerState.ZoneIdToState.Keys.Max() : 0) + 1;
            }

            return zone;
        }
    }
}
