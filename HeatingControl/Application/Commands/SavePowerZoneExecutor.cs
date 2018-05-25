using System;
using System.Collections.Generic;
using System.Linq;
using Commons;
using Commons.Extensions;
using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public interface ISavePowerZoneExecutor
    {
        void Execute(SavePowerZoneExecutorInput input, Building buidling, ControllerState controllerState);
    }

    public class SavePowerZoneExecutorInput
    {
        public int? PowerZoneId { get; set; }
        public string Name { get; set; }
        public ICollection<int> AffectedHeatersIds { get; set; }
        public float PowerLimitValue { get; set; }
        public UsageUnit PowerLimitUnit { get; set; }
        public int RoundRobinIntervalMinutes { get; set; }
    }

    public class SavePowerZoneExecutor : ISavePowerZoneExecutor
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public SavePowerZoneExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public void Execute(SavePowerZoneExecutorInput input, Building buidling, ControllerState controllerState)
        {
            if (!InputIsValid(input, controllerState))
            {
                return;
            }

            PowerZone existingPowerZone = null;

            if (input.PowerZoneId.HasValue)
            {
                var existingPowerZoneId = input.PowerZoneId.Value;
                existingPowerZone = controllerState.PowerZoneIdToState[existingPowerZoneId].PowerZone;

                controllerState.PowerZoneIdToState.Remove(existingPowerZoneId);
                buidling.PowerZones.Remove(x => x.PowerZoneId == existingPowerZoneId);
            }

            var powerZone = BuildNewPowerZone(input, existingPowerZone, controllerState);

            controllerState.PowerZoneIdToState.Add(powerZone.PowerZoneId,
                                                   new PowerZoneState
                                                   {
                                                       PowerZone = powerZone
                                                   });

            buidling.PowerZones.Add(powerZone);
            _buildingModelSaver.Save(buidling);
        }

        private bool InputIsValid(SavePowerZoneExecutorInput input, ControllerState controllerState)
        {
            if (input.Name.IsNullOrEmpty())
            {
                Logger.Warning("Cannot add zone without name.");
                return false;
            }

            if (input.PowerZoneId.HasValue && !controllerState.PowerZoneIdToState.ContainsKey(input.PowerZoneId.Value))
            {
                Logger.Warning("Unknown power zone id {0}.", new object[] { input.PowerZoneId.Value });
                return false;
            }

            if (input.RoundRobinIntervalMinutes < 1)
            {
                Logger.Warning("Cannot set power zone interval to less than 1 minute.");
                return false;
            }

            var highestHeaterPower = 0f;

            foreach (var heaterId in input.AffectedHeatersIds)
            {
                var heater = controllerState.HeaterIdToState.GetValueOrDefault(heaterId);

                if (heater == null)
                {
                    Logger.Warning("Heater of id {0} not found.", new object[] { heaterId });
                    return false;

                }

                if (heater.Heater.UsageUnit != input.PowerLimitUnit)
                {
                    Logger.Warning("Power unit missmatch for heater of id {0} when creating new power zone.", new object[] { heaterId });
                    return false;
                }

                if (controllerState.PowerZoneIdToState
                                   .Select(z => z.Value.PowerZone)
                                   .Where(x => !input.PowerZoneId.HasValue || input.PowerZoneId.Value != x.PowerZoneId)
                                   .Any(z => z.HeaterIds.Contains(heaterId)))
                {
                    Logger.Warning("Heater of id {0} is already in use by another power zone.", new object[] { heaterId });
                    return false;
                }

                highestHeaterPower = Math.Max(highestHeaterPower, heater.Heater.UsagePerHour);
            }

            if (input.PowerLimitValue < highestHeaterPower)
            {
                Logger.Warning("Cannot add power zone with total limit less than max usage from selected heaters.");
                return false;
            }

            return true;
        }


        private static PowerZone BuildNewPowerZone(SavePowerZoneExecutorInput input, PowerZone existingPowerZone, ControllerState controllerState)
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
