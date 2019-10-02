using System.Collections.Generic;
using System.Linq;
using Commons;
using Commons.Extensions;
using Commons.Localization;
using Domain;
using HeatingControl.Application.DataAccess;
using HeatingControl.Extensions;
using HeatingControl.Models;

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
        private readonly IRepository<Zone> _zoneRepository;
        private readonly IRepository<Heater> _heaterRepository;

        public SaveZoneCommandExecutor(IRepository<Zone> zoneRepository,
                                       IRepository<Heater> heaterRepository)
        {
            _zoneRepository = zoneRepository;
            _heaterRepository = heaterRepository;
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

                foreach (var heaterToDisable in zoneState.Zone.Heaters)
                {
                    if (command.HeaterIds.Contains(heaterToDisable.HeaterId))
                    {
                        continue;
                    }

                    context.ControllerState.HeaterIdToState[heaterToDisable.HeaterId].OutputState = false;
                }
            }

            if (existingZone == null)
            {
                var zone = BuildNewZone(command, existingZone);

                existingZone = _zoneRepository.Create(zone);
                context.ControllerState.Model.Zones.Add(existingZone);

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
            }
            else
            {
                _zoneRepository.Update(existingZone);
            }

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
                                   .Any(z => z.Heaters.FirstOrDefault(h => h.HeaterId == heaterId) != null))
                {
                    return CommandResult.WithValidationError(Localization.ValidationMessage.HeaterAlreadyInUseByAnotherZone.FormatWith(heaterId));
                }
            }

            return null;
        }

        private static Zone BuildNewZone(SaveZoneCommand command, Zone existingZone)
        {
            var zone = new Zone
            {
                Name = command.Name
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
                if (existingZone.IsTemperatureControlled() == zone.IsTemperatureControlled())
                {
                    zone.Schedule = existingZone.Schedule;
                }
                else
                {
                    Logger.Info(Localization.NotificationMessage.ScheduledRemovedDueToControlTypeChange.FormatWith(command.Name));
                }
            }

            return zone;
        }
    }
}
