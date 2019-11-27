using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using HeatingControl.Application.DataAccess.PowerZone;
using HeatingControl.Models;

namespace HeatingControl.Application.Commands
{
    public class SavePowerZoneCommand : PowerZoneSaverInput { }

    public class SavePowerZoneCommandExecutor : ICommandExecutor<SavePowerZoneCommand>
    {
        private readonly IPowerZoneSaver _powerZoneSaver;

        public SavePowerZoneCommandExecutor(IPowerZoneSaver powerZoneSaver)
        {
            _powerZoneSaver = powerZoneSaver;
        }

        public CommandResult Execute(SavePowerZoneCommand command, CommandContext context)
        {
            var validationResult = Validate(command, context.ControllerState);

            if (validationResult != null)
            {
                return validationResult;
            }

            var powerZone = _powerZoneSaver.Save(command, context.ControllerState.Model);

            var powerZoneState = context.ControllerState.PowerZoneIdToState.GetValueOrDefault(powerZone.PowerZoneId);

            if (powerZoneState == null)
            {
                context.ControllerState.PowerZoneIdToState.Add(powerZone.PowerZoneId,
                                                               new PowerZoneState
                                                               {
                                                                   PowerZone = powerZone
                                                               });
            }
            else
            {
                powerZoneState.PowerZone = powerZone;
            }

            return CommandResult.Empty;
        }

        private CommandResult Validate(SavePowerZoneCommand command, ControllerState controllerState)
        {
            if (command.Name.IsNullOrEmpty())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.NameCantBeEmpty);
            }

            if (command.PowerLimitValue < 0m)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.PowerLimitCantBeNegative);
            }

            if (command.PowerZoneId.HasValue && !controllerState.PowerZoneIdToState.ContainsKey(command.PowerZoneId.Value))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownPowerZoneId.FormatWith(command.PowerZoneId.Value));
            }

            if (command.RoundRobinIntervalMinutes < 1)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.PowerZoneIntervalCantBeLessThan1Minute);
            }

            if (command.SwitchDelay < 0)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.PowerZoneSwitchDelayCantBeNegative);
            }

            var highestHeaterPower = 0m;

            foreach (var heaterId in command.AffectedHeatersIds)
            {
                var heater = controllerState.HeaterIdToState.GetValueOrDefault(heaterId);

                if (heater == null)
                {
                    return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownHeaterId.FormatWith(heaterId));

                }

                if (heater.Heater.UsageUnit != command.PowerLimitUnit)
                {
                    return CommandResult.WithValidationError(Localization.ValidationMessage.PowerZoneHeaterUnitMismatch.FormatWith(heaterId));
                }

                if (controllerState.PowerZoneIdToState
                                   .Select(z => z.Value.PowerZone)
                                   .Where(x => !command.PowerZoneId.HasValue || command.PowerZoneId.Value != x.PowerZoneId)
                                   .Any(z => z.Heaters.Contains(heater.Heater)))
                {
                    return CommandResult.WithValidationError(Localization.ValidationMessage.HeaterAlreadyInUseByAnotherPowerZone.FormatWith(heaterId));
                }

                highestHeaterPower = Math.Max(highestHeaterPower, heater.Heater.UsagePerHour);
            }

            if (command.PowerLimitValue < highestHeaterPower)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.PowerZoneTotalLimitLessThanTopHeaterUsage);
            }

            return null;
        }
    }
}
