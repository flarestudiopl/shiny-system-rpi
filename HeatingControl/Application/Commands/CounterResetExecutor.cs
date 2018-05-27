using System;
using HeatingControl.Models;
using Storage.StorageDatabase.Counter;
using Commons.Extensions;

namespace HeatingControl.Application.Commands
{
    public interface ICounterResetExecutor
    {
        void Execute(int zoneId, int userId, ControllerState controllerState);
    }

    public class CounterResetExecutor : ICounterResetExecutor
    {
        private readonly ICounterResetter _counterResetter;
        private readonly ICounterAccumulator _counterAccumulator;

        public CounterResetExecutor(ICounterResetter counterResetter,
                                    ICounterAccumulator counterAccumulator)
        {
            _counterResetter = counterResetter;
            _counterAccumulator = counterAccumulator;
        }

        public void Execute(int zoneId, int userId, ControllerState controllerState)
        {
            var zone = controllerState.ZoneIdToState.GetValueOrDefault(zoneId);

            if (zone == null)
            {
                return;
            }

            foreach (var heaterId in zone.Zone.HeaterIds)
            {
                controllerState.HeaterIdToState[heaterId].LastCounterStart = DateTime.Now;

                _counterResetter.Reset(new CounterResetterInput
                {
                    HeaterId = heaterId,
                    UserId = userId
                });

                _counterAccumulator.Accumulate(new CounterAccumulatorInput
                {
                    HeaterId = heaterId,
                    SecondsToAccumulate = 0
                });
            }
        }
    }
}
