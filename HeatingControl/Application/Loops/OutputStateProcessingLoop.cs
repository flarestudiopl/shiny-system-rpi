using System;
using System.Threading;
using Commons;
using Commons.Extensions;
using Commons.Localization;
using HeatingControl.Application.Loops.Processing;
using Domain;
using HeatingControl.Extensions;
using HeatingControl.Models;
using System.Linq;

namespace HeatingControl.Application.Loops
{
    public interface IOutputStateProcessingLoop
    {
        void Start(ControllerState controllerState, CancellationToken cancellationToken);
    }

    public class OutputStateProcessingLoop : IOutputStateProcessingLoop
    {
        private readonly IZoneTemperatureProvider _zoneTemperatureProvider;
        private readonly IHysteresisProcessor _hysteresisProcessor;
        private readonly IPowerZoneOutputLimiter _powerZoneOutputLimiter;
        private readonly IOutputsWriter _outputsWriter;

        public OutputStateProcessingLoop(IZoneTemperatureProvider zoneTemperatureProvider,
                                         IHysteresisProcessor hysteresisProcessor,
                                         IPowerZoneOutputLimiter powerZoneOutputLimiter,
                                         IOutputsWriter outputsWriter)
        {
            _zoneTemperatureProvider = zoneTemperatureProvider;
            _hysteresisProcessor = hysteresisProcessor;
            _powerZoneOutputLimiter = powerZoneOutputLimiter;
            _outputsWriter = outputsWriter;
        }

        public void Start(ControllerState controllerState, CancellationToken cancellationToken)
        {
            Loop.Start("Output state",
                       controllerState.Model.ControlLoopIntervalSecondsMilliseconds,
                       () =>
                       {
                           ProcessZones(controllerState);
                           ProcessHeaters(controllerState);
                           ProcessPowerZones(controllerState);

                           _outputsWriter.Write(controllerState, false);
                       },
                       cancellationToken);
        }

        private void ProcessZones(ControllerState controllerState)
        {
            foreach (var zoneState in controllerState.ZoneIdToState.Values)
            {
                zoneState.EnableOutputs = zoneState.Zone.IsTemperatureControlled()
                                              ? ProcessTemperatureBasedOutput(controllerState, zoneState)
                                              : ProcessOnOffBasedOutput(zoneState);
            }
        }

        private bool ProcessTemperatureBasedOutput(ControllerState controllerState, ZoneState zoneState)
        {
            var temperatureData = _zoneTemperatureProvider.Provide(zoneState.Zone.ZoneId, controllerState);

            if (temperatureData == null)
            {
                Logger.Warning(Localization.NotificationMessage.NoTemperatureData.FormatWith(zoneState.Zone.TemperatureControlledZone.TemperatureSensorId, zoneState.Zone.Name));

                return false;
            }

            var outputState = zoneState.EnableOutputs;

            if (DateTime.UtcNow - temperatureData.LastRead < TimeSpan.FromMinutes(5))
            {
                var setPoint = 0f;

                switch (zoneState.ControlMode)
                {
                    case ZoneControlMode.LowOrDisabled:
                        setPoint = zoneState.Zone.TemperatureControlledZone.LowSetPoint;
                        break;
                    case ZoneControlMode.HighOrEnabled:
                        setPoint = zoneState.Zone.TemperatureControlledZone.HighSetPoint;
                        break;
                    case ZoneControlMode.Schedule:
                        if (zoneState.ScheduleState.DesiredTemperature.HasValue)
                        {
                            setPoint = zoneState.ScheduleState.DesiredTemperature.Value;
                        }

                        break;
                }

                outputState = _hysteresisProcessor.Process(temperatureData.AverageTemperature,
                                                           outputState,
                                                           setPoint,
                                                           zoneState.Zone.TemperatureControlledZone.Hysteresis);
            }
            else
            {
                outputState = false;

                Logger.Warning(Localization.NotificationMessage.TemperatureValueTooOld.FormatWith(zoneState.Zone.Name));
            }

            return outputState;
        }

        private bool ProcessOnOffBasedOutput(ZoneState zoneState)
        {
            var outputState = zoneState.EnableOutputs;

            switch (zoneState.ControlMode)
            {
                case ZoneControlMode.LowOrDisabled:
                    outputState = false;
                    break;
                case ZoneControlMode.HighOrEnabled:
                    outputState = true;
                    break;
                case ZoneControlMode.Schedule:
                    if (zoneState.ScheduleState.HeatingEnabled.HasValue)
                    {
                        outputState = zoneState.ScheduleState.HeatingEnabled.Value;
                    }

                    break;
            }

            return outputState;
        }

        private void ProcessHeaters(ControllerState controllerState)
        {
            foreach (var zone in controllerState.ZoneIdToState.Values)
            {
                foreach (var heater in zone.Zone.Heaters.Select(x => x.HeaterId))
                {
                    controllerState.HeaterIdToState[heater].OutputState = zone.EnableOutputs;
                }
            }
        }

        private void ProcessPowerZones(ControllerState controllerState)
        {
            foreach (var powerZone in controllerState.PowerZoneIdToState.Values)
            {
                _powerZoneOutputLimiter.Limit(powerZone, controllerState);
            }
        }
    }
}
