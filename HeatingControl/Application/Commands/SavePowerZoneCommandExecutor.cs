using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Domain;
using HeatingControl.Application.DataAccess;
using HeatingControl.Models;

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
        private readonly IRepository<PowerZone> _powerZoneRepository;
        private readonly IRepository<Heater> _heaterRepository;

        public SavePowerZoneCommandExecutor(IRepository<PowerZone> powerZoneRepository,
                                            IRepository<Heater> heaterRepository)
        {
            _powerZoneRepository = powerZoneRepository;
            _heaterRepository = heaterRepository;
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

            var powerZone = new PowerZone
            {
                Name = command.Name,
                UsageUnit = command.PowerLimitUnit,
                MaxUsage = command.PowerLimitValue,
                RoundRobinIntervalMinutes = command.RoundRobinIntervalMinutes
            };

            powerZone = _powerZoneRepository.Create(powerZone);

            foreach (var heaterId in command.AffectedHeatersIds)
            {
                var heater = _heaterRepository.ReadSingle(x=>x.HeaterId == heaterId);
                heater.PowerZone = powerZone;
                heater.PowerZoneId = powerZone.PowerZoneId;
                _heaterRepository.Update(heater);
            }

            context.ControllerState.PowerZoneIdToState.Add(powerZone.PowerZoneId,
                                                           new PowerZoneState
                                                           {
                                                               PowerZone = powerZone
                                                           });

            context.ControllerState.Model.PowerZones.Add(powerZone);

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
