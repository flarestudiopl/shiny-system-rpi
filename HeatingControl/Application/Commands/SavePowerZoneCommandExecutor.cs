using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public class SavePowerZoneCommand
    {
        public int? PowerZoneId { get; set; }
        public string Name { get; set; }
        public ICollection<int> AffectedHeatersIds { get; set; }
        public decimal PowerLimitValue { get; set; }
        public UsageUnit PowerLimitUnit { get; set; }
        public int RoundRobinIntervalMinutes { get; set; }
    }

    public class SavePowerZoneCommandExecutor : ICommandExecutor<SavePowerZoneCommand>
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public SavePowerZoneCommandExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public CommandResult Execute(SavePowerZoneCommand command, CommandContext context)
        {
            var validationResult = Validate(command, context.ControllerState);

            if (validationResult != null)
            {
                return validationResult;
            }

            PowerZone existingPowerZone = null;

            if (command.PowerZoneId.HasValue)
            {
                var existingPowerZoneId = command.PowerZoneId.Value;
                existingPowerZone = context.ControllerState.PowerZoneIdToState[existingPowerZoneId].PowerZone;

                context.ControllerState.PowerZoneIdToState.Remove(existingPowerZoneId);
                context.ControllerState.Model.PowerZones.Remove(x => x.PowerZoneId == existingPowerZoneId);
            }

            var powerZone = BuildNewPowerZone(command, existingPowerZone, context.ControllerState);

            context.ControllerState.PowerZoneIdToState.Add(powerZone.PowerZoneId,
                                                           new PowerZoneState
                                                           {
                                                               PowerZone = powerZone
                                                           });

            context.ControllerState.Model.PowerZones.Add(powerZone);
            _buildingModelSaver.Save(context.ControllerState.Model);

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
                                   .Any(z => z.HeaterIds.Contains(heaterId)))
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

        private static PowerZone BuildNewPowerZone(SavePowerZoneCommand input, PowerZone existingPowerZone, ControllerState controllerState)
        {
            var powerZone = new PowerZone
                            {
                                Name = input.Name,
                                HeaterIds = input.AffectedHeatersIds.ToHashSet(),
                                UsageUnit = input.PowerLimitUnit,
                                MaxUsage = input.PowerLimitValue,
                                RoundRobinIntervalMinutes = input.RoundRobinIntervalMinutes,
                                PowerZoneId = existingPowerZone?.PowerZoneId ??
                                              (controllerState.PowerZoneIdToState.Keys.Any() ? controllerState.PowerZoneIdToState.Keys.Max() : 0) + 1
                            };

            return powerZone;
        }
    }
}
