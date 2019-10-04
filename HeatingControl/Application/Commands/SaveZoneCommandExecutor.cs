using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Domain;
using HeatingControl.Application.DataAccess.Zone;
using HeatingControl.Models;

namespace HeatingControl.Application.Commands
{
    public class SaveZoneCommand : ZoneSaverInput { }

    public class SaveZoneCommandExecutor : ICommandExecutor<SaveZoneCommand>
    {
        private readonly IZoneSaver _zoneSaver;

        public SaveZoneCommandExecutor(IZoneSaver zoneSaver)
        {
            _zoneSaver = zoneSaver;
        }

        public CommandResult Execute(SaveZoneCommand command, CommandContext context)
        {
            var validationResult = Validate(command, context.ControllerState);

            if (validationResult != null)
            {
                return validationResult;
            }

            DisableRemovedHeaters(command, context);

            var savedZone = _zoneSaver.Save(command, context.ControllerState.Model);

            var zoneState = context.ControllerState.ZoneIdToState.GetValueOrDefault(savedZone.ZoneId);

            if (zoneState == null)
            {
                context.ControllerState.ZoneIdToState.Add(savedZone.ZoneId,
                                          new ZoneState
                                          {
                                              Zone = savedZone,
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
                zoneState.Zone = savedZone;
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

                var heater = controllerState.HeaterIdToState[heaterId].Heater;

                if ((heater.ZoneId.HasValue && !command.ZoneId.HasValue) ||
                    (heater.ZoneId.HasValue && command.ZoneId.Value != heater.ZoneId.Value))
                {
                    return CommandResult.WithValidationError(Localization.ValidationMessage.HeaterAlreadyInUseByAnotherZone.FormatWith(heaterId));
                }
            }

            return null;
        }

        private static void DisableRemovedHeaters(SaveZoneCommand command, CommandContext context)
        {
            if (command.ZoneId.HasValue)
            {
                var zone = context.ControllerState.ZoneIdToState.GetValueOrDefault(command.ZoneId.Value);

                if (zone != null)
                {
                    foreach (var heaterToRemove in zone.Zone.Heaters.Where(x => !command.HeaterIds.Contains(x.HeaterId)))
                    {
                        context.ControllerState.HeaterIdToState[heaterToRemove.HeaterId].OutputState = false;
                    }
                }
            }
        }
    }
}
