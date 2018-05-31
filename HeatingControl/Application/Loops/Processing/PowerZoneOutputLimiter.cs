using System;
using System.Linq;
using HeatingControl.Models;

namespace HeatingControl.Application.Loops.Processing
{
    public interface IPowerZoneOutputLimiter
    {
        void Limit(PowerZoneState powerZoneState, ControllerState controllerState);
    }

    public class PowerZoneOutputLimiter : IPowerZoneOutputLimiter
    {
        public void Limit(PowerZoneState powerZoneState, ControllerState controllerState)
        {
            var availablePower = powerZoneState.PowerZone.MaxUsage;

            var requiredHeaterIdToUsage = controllerState.HeaterIdToState
                                                         .Where(x => powerZoneState.PowerZone.HeaterIds.Contains(x.Key) &&
                                                                     controllerState.HeaterIdToState[x.Key].OutputState)
                                                         .ToDictionary(x => x.Key,
                                                                       x => x.Value.Heater.UsagePerHour);

            if (requiredHeaterIdToUsage.Values.Sum(x => x) <= availablePower)
            {
                return;
            }

            var now = DateTime.Now;

            var heatersThatCantBeDisabled = controllerState.HeaterIdToState
                                                           .Where(x => x.Value.OutputState &&
                                                                       (now - x.Value.LastStateChange).TotalSeconds <= x.Value.Heater.MinimumStateChangeIntervalSeconds);

            var totalPower = 0f;

            foreach (var heater in heatersThatCantBeDisabled)
            {
                totalPower += heater.Value.Heater.UsagePerHour;
                requiredHeaterIdToUsage.Remove(heater.Key);
            }

            for (var i = 0; i < requiredHeaterIdToUsage.Count; i++)
            {
                var heaterIndex = (i + powerZoneState.NextIntervalOffset) % requiredHeaterIdToUsage.Count;
                var heater = requiredHeaterIdToUsage.ElementAt(heaterIndex);

                if (totalPower + heater.Value <= availablePower)
                {
                    totalPower += heater.Value;
                }
                else
                {
                    controllerState.HeaterIdToState[heater.Key].OutputState = false;
                }
            }

            if (powerZoneState.NextIntervalIncrementationDateTime < now)
            {
                unchecked
                {
                    powerZoneState.NextIntervalOffset++;
                }

                powerZoneState.NextIntervalIncrementationDateTime = now.AddMinutes(powerZoneState.PowerZone.RoundRobinIntervalMinutes);
            }
        }
    }
}
