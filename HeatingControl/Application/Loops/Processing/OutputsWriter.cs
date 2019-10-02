using System;
using HardwareAccess.Devices;
using HeatingControl.Models;

namespace HeatingControl.Application.Loops.Processing
{
    public interface IOutputsWriter
    {
        void Write(ControllerState controllerState, bool forceImmidiateAction);
    }

    public class OutputsWriter : IOutputsWriter
    {
        private readonly IPowerOutputProvider _powerOutputProvider;
        private readonly IUsageCollector _usageCollector;

        public OutputsWriter(IPowerOutputProvider powerOutputProvider,
                             IUsageCollector usageCollector)
        {
            _powerOutputProvider = powerOutputProvider;
            _usageCollector = usageCollector;
        }

        public void Write(ControllerState controllerState, bool forceImmidiateAction)
        {
            var now = DateTime.UtcNow;

            foreach (var heater in controllerState.HeaterIdToState.Values)
            {
                if (CanSwitchState(now, heater, forceImmidiateAction) &&
                    StateShouldBeUpdated(heater))
                {
                    _powerOutputProvider.Provide(heater.Heater.DigitalOutput.ProtocolName)
                                        .SetState(heater.Heater.DigitalOutput.DeviceId, heater.Heater.DigitalOutput.OutputChannel, heater.OutputState);

                    _usageCollector.Collect(heater, controllerState);

                    heater.LastStateChange = now;
                }
            }
        }

        private static bool CanSwitchState(DateTime now, HeaterState heater, bool forceImmidiateAction)
        {
            return forceImmidiateAction ||
                   (now - heater.LastStateChange).TotalSeconds > heater.Heater.MinimumStateChangeIntervalSeconds;
        }

        private bool StateShouldBeUpdated(HeaterState heater)
        {
            return heater.OutputState != _powerOutputProvider.Provide(heater.Heater.DigitalOutput.ProtocolName)
                                                             .GetState(heater.Heater.DigitalOutput.DeviceId, heater.Heater.DigitalOutput.OutputChannel);
        }
    }
}
