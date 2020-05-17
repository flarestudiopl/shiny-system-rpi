using System;
using System.Linq;
using HeatingControl.Application.DataAccess.Counter;
using HeatingControl.Models;

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

            if (!heaterState.OutputState)
            {
                _counterAccumulator.Accumulate(new CounterAccumulatorInput
                {
                    HeaterId = heater.HeaterId,
                    SecondsToAccumulate = (int)(DateTime.UtcNow - heaterState.LastCounterStart).TotalSeconds
                });
            }
            
            heaterState.LastCounterStart = DateTime.UtcNow;

            controllerState.InstantUsage = controllerState.HeaterIdToState
                                                          .Values
                                                          .GroupBy(x => x.Heater.UsageUnit)
                                                          .ToDictionary(x => x.Key,
                                                                        x => x.Where(h => h.OutputState)
                                                                              .Sum(h => h.Heater.UsagePerHour));
        }
    }
}
