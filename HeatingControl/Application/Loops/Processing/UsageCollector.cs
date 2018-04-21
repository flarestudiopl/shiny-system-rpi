using HeatingControl.Models;

namespace HeatingControl.Application.Loops.Processing
{
    public interface IUsageCollector
    {
        void Collect(HeaterState heaterState, ControllerState controllerState);
    }

    public class UsageCollector : IUsageCollector
    {
        public void Collect(HeaterState heaterState, ControllerState controllerState)
        {
            var heater = heaterState.Heater;

            if (heaterState.OutputState)
            {
                if (!controllerState.InstantUsage.ContainsKey(heater.UsageUnit))
                {
                    controllerState.InstantUsage.Add(heater.UsageUnit, heater.UsagePerHour);
                }
                else
                {
                    controllerState.InstantUsage[heater.UsageUnit] += heater.UsagePerHour;
                }
            }
            else
            {
                if (!controllerState.InstantUsage.ContainsKey(heater.UsageUnit))
                {
                    controllerState.InstantUsage.Add(heater.UsageUnit, 0f);
                }
                else
                {
                    controllerState.InstantUsage[heater.UsageUnit] -= heater.UsagePerHour;
                }
            }
        }
    }
}
