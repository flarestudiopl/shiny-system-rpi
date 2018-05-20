using System;
using System.Linq;
using HeatingControl.Models;

namespace HeatingControl.Application.Loops.Processing
{
    public interface IPowerZoneOutputAllowanceCalculator
    {
        void Calculate(PowerZoneState powerZoneState, ControllerState controllerState);
    }

    public class PowerZoneOutputAllowanceCalculator : IPowerZoneOutputAllowanceCalculator
    {
        public void Calculate(PowerZoneState powerZoneState, ControllerState controllerState)
        {
            var availablePower = powerZoneState.PowerZone.MaxUsage;

            var heaterIdToRequiredPower = controllerState.HeaterIdToState
                                                         .Where(x => powerZoneState.HeaterIdToPowerOnAllowance.ContainsKey(x.Key))
                                                         .ToDictionary(x => x.Key,
                                                                       x => x.Value.Heater.UsagePerHour);

            if (heaterIdToRequiredPower.Values.Sum(x => x) <= availablePower)
            {
                foreach (var allowanceKey in powerZoneState.HeaterIdToPowerOnAllowance.Keys.ToList())
                {
                    powerZoneState.HeaterIdToPowerOnAllowance[allowanceKey] = true;
                }
            }
            else
            {
                var totalPower = 0f;

                for (var i = 0; i < powerZoneState.HeaterIdToPowerOnAllowance.Count; i++)
                {
                    var heaterIndex = (i + powerZoneState.NextAllowanceRecalculationOffset) % powerZoneState.HeaterIdToPowerOnAllowance.Count;
                    var heater = heaterIdToRequiredPower.ElementAt(heaterIndex);

                    if (totalPower + heater.Value <= availablePower)
                    {
                        totalPower += heater.Value;
                        powerZoneState.HeaterIdToPowerOnAllowance[heater.Key] = true;
                    }
                    else
                    {
                        powerZoneState.HeaterIdToPowerOnAllowance[heater.Key] = false;
                    }
                }

                powerZoneState.NextAllowanceRecalculationOffset = (powerZoneState.NextAllowanceRecalculationOffset + 1) % powerZoneState.HeaterIdToPowerOnAllowance.Count;
            }

            powerZoneState.NextAllowanceRecalculationDateTime = DateTime.Now.AddMinutes(powerZoneState.PowerZone.RoundRobinIntervalMinutes);
        }
    }
}
