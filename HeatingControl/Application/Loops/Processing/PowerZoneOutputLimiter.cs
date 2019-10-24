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

            var requiredHeaterIdToUsage = powerZoneState.PowerZone
                                                        .Heaters
                                                        .Where(h => controllerState.HeaterIdToState[h.HeaterId].OutputState)
                                                        .ToDictionary(h => h.HeaterId,
                                                                      h => h.UsagePerHour);

            if (requiredHeaterIdToUsage.Values.Sum(x => x) <= availablePower)
            {
                return;
            }

            var now = DateTime.UtcNow;

            if (powerZoneState.NextIntervalIncrementationDateTime < now)
            {
                unchecked
                {
                    powerZoneState.NextIntervalOffset++;
                }

                powerZoneState.NextIntervalIncrementationDateTime = now.AddMinutes(powerZoneState.PowerZone.RoundRobinIntervalMinutes);
            }

            var heatersThatCantBeDisabled = powerZoneState.PowerZone
                                                          .Heaters
                                                          .Where(h =>
                                                             {
                                                                 var heaterState = controllerState.HeaterIdToState[h.HeaterId];
                                                                 return heaterState.OutputState &&
                                                                        (now - heaterState.LastStateChange).TotalSeconds <= h.MinimumStateChangeIntervalSeconds;
                                                             })
                                                          .ToDictionary(h => h.HeaterId); ;

            var totalPower = 0m;

            foreach (var heater in heatersThatCantBeDisabled)
            {
                totalPower += heater.Value.UsagePerHour;
            }

            for (var i = 0; i < requiredHeaterIdToUsage.Count; i++)
            {
                var heaterIndex = (i + powerZoneState.NextIntervalOffset) % requiredHeaterIdToUsage.Count;
                var heater = requiredHeaterIdToUsage.ElementAt(heaterIndex);

                if (totalPower + heater.Value <= availablePower)
                {
                    totalPower += heater.Value;
                }
                else if (!heatersThatCantBeDisabled.ContainsKey(heater.Key))
                {
                    controllerState.HeaterIdToState[heater.Key].OutputState = false;
                }
            }

        }
    }
}
