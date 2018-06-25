using System;
using HeatingControl.Models;
using Storage.StorageDatabase.Counter;

namespace HeatingControl.Application.Loops.Processing
{
    public interface IUsageCollector
    {
        void Collect(HeaterState heaterState, ControllerState controllerState);
    }

    public class UsageCollector : IUsageCollector
    {
        private readonly ICounterAccumulator _counterAccumulator;

        public UsageCollector(ICounterAccumulator counterAccumulator)
        {
            _counterAccumulator = counterAccumulator;
        }

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

                heaterState.LastCounterStart = DateTime.Now;
            }
            else
            {
                if (!controllerState.InstantUsage.ContainsKey(heater.UsageUnit))
                {
                    controllerState.InstantUsage.Add(heater.UsageUnit, 0m);
                }
                else
                {
                    controllerState.InstantUsage[heater.UsageUnit] -= heater.UsagePerHour;
                }

                _counterAccumulator.Accumulate(new CounterAccumulatorInput
                                               {
                                                   HeaterId = heater.HeaterId,
                                                   SecondsToAccumulate = (int)(DateTime.Now - heaterState.LastCounterStart).TotalSeconds
                                               });
            }
        }
    }
}
