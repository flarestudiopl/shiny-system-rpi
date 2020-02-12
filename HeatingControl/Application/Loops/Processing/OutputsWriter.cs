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
                PowerZoneState heaterPowerZoneState = null;

                if (heater.Heater.PowerZoneId.HasValue)
                {
                    heaterPowerZoneState = controllerState.PowerZoneIdToState[heater.Heater.PowerZoneId.Value];
                }

                if (CanSwitchState(now, heater, heaterPowerZoneState, forceImmidiateAction) &&
                    StateShouldBeUpdated(heater))
                {
                    _powerOutputProvider.Provide(heater.Heater.DigitalOutput.ProtocolName)
                                        .SetState(heater.Heater.DigitalOutput.OutputDescriptor, heater.OutputState);

                    _usageCollector.Collect(heater, controllerState);

                    heater.LastStateChange = now;

                    if (heaterPowerZoneState != null)
                    {
                        heaterPowerZoneState.LastOutputStateChange = now;
                    }
                }
            }
        }

        private static bool CanSwitchState(DateTime now, HeaterState heater, PowerZoneState heaterPowerZoneState, bool forceImmidiateAction)
        {
            if (forceImmidiateAction)
            {
                return true;
            }

            return (now - heater.LastStateChange).TotalSeconds >= heater.Heater.MinimumStateChangeIntervalSeconds &&
                   (heaterPowerZoneState == null || (now - heaterPowerZoneState.LastOutputStateChange).TotalSeconds >= heaterPowerZoneState.PowerZone.SwitchDelayBetweenOutputsSeconds);
        }

        private bool StateShouldBeUpdated(HeaterState heater)
        {
            return heater.OutputState != _powerOutputProvider.Provide(heater.Heater.DigitalOutput.ProtocolName)
                                                             .GetState(heater.Heater.DigitalOutput.OutputDescriptor);
        }
    }
}
