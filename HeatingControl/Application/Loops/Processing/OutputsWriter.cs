using System;
using Commons;
using Commons.Extensions;
using Commons.Localization;
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
                    var setStateSuccess = _powerOutputProvider.Provide(heater.Heater.DigitalOutput.ProtocolName)
                                                              .TrySetState(heater.Heater.DigitalOutput.OutputDescriptor, heater.OutputState);

                    if (setStateSuccess)
                    {
                        _usageCollector.Collect(heater, controllerState);

                        heater.StateChangeFailureSince = null;
                        heater.LastStateChange = now;

                        if (heaterPowerZoneState != null)
                        {
                            heaterPowerZoneState.LastOutputStateChange = now;
                        }
                    }
                    else
                    {
                        heater.StateChangeFailureSince = heater.StateChangeFailureSince ?? now;

                        if (now - heater.StateChangeFailureSince > TimeSpan.FromSeconds(30))
                        {
                            Logger.Warning(Localization.NotificationMessage.OutputWriteFailed.FormatWith(heater.Heater.Name));
                            heater.StateChangeFailureSince = now;
                        }
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
            var currentState = _powerOutputProvider.Provide(heater.Heater.DigitalOutput.ProtocolName)
                                                   .TryGetState(heater.Heater.DigitalOutput.OutputDescriptor);

            return !currentState.HasValue || heater.OutputState != currentState.Value;
        }
    }
}
