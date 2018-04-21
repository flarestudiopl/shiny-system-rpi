﻿using System;
using System.Threading;
using Commons;
using HardwareAccess.Devices;
using HeatingControl.Domain;
using HeatingControl.Extensions;
using HeatingControl.Models;

namespace HeatingControl.Application.Loops
{
    public interface IOutputStateProcessingLoop
    {
        void Start(int intervalMilliseconds, ControllerState controllerState, CancellationToken cancellationToken);
    }

    public class OutputStateProcessingLoop : IOutputStateProcessingLoop
    {
        private readonly IPowerOutput _powerOutput;
        private readonly IZoneTemperatureProvider _zoneTemperatureProvider;
        private readonly IHysteresisProcessor _hysteresisProcessor;

        public OutputStateProcessingLoop(IPowerOutput powerOutput,
                                         IZoneTemperatureProvider zoneTemperatureProvider,
                                         IHysteresisProcessor hysteresisProcessor)
        {
            _powerOutput = powerOutput;
            _zoneTemperatureProvider = zoneTemperatureProvider;
            _hysteresisProcessor = hysteresisProcessor;
        }

        public void Start(int intervalMilliseconds, ControllerState controllerState, CancellationToken cancellationToken)
        {
            LoopHelper.Start("Output state",
                             intervalMilliseconds,
                             () =>
                             {
                                 ProcessZones(controllerState);
                                 ProcessHeaters(controllerState);
                                 ProcessPowerZones(controllerState);

                                 WriteOutputs(controllerState);
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
            ;
            if (temperatureData == null)
            {
                Logger.Warning("No temperature data for sensor {0} in zone {1}. Proactive power cutoff.",
                               new object[] { zoneState.Zone.TemperatureControlledZone.TemperatureSensorId, zoneState.Zone.Name });

                return false;
            }

            var outputState = zoneState.EnableOutputs;

            if (DateTime.Now - temperatureData.LastRead < TimeSpan.FromMinutes(5))
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
                        setPoint = zoneState.ScheduleState.DesiredTemperature.Value;
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
                Logger.Warning("Temperature value for zone {0} is too old. Proactive power cutoff.",
                               new object[] { zoneState.Zone.Name });
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
                    outputState = zoneState.ScheduleState.HeatingEnabled.Value;
                    break;
            }

            return outputState;
        }

        private void ProcessHeaters(ControllerState controllerState)
        {
            foreach (var zone in controllerState.ZoneIdToState.Values)
            {
                foreach (var heater in zone.Zone.HeaterIds)
                {
                    controllerState.HeaterIdToState[heater].OutputState = zone.EnableOutputs;
                }
            }
        }

        private void ProcessPowerZones(ControllerState controllerState)
        {
            // TODO: apply power limits
        }

        private void WriteOutputs(ControllerState controllerState)
        {
            var now = DateTime.Now;

            foreach (var heater in controllerState.HeaterIdToState.Values)
            {
                var powerOutput = heater.Heater.PowerOutput;

                if ((now - heater.LastStateChange).TotalSeconds > heater.Heater.MinimumStateChangeIntervalSeconds &&
                    heater.OutputState != _powerOutput.GetState(powerOutput.PowerOutputDeviceId, powerOutput.PowerOutputChannel))
                {
                    _powerOutput.SetState(powerOutput.PowerOutputDeviceId, powerOutput.PowerOutputChannel, heater.OutputState);
                    heater.LastStateChange = now;
                    
                    if (heater.OutputState) // TODO - move out and add no key in dict handling
                    {
                        controllerState.InstantUsage[heater.Heater.UsageUnit] += heater.Heater.UsagePerHour;
                    }
                    else
                    {
                        controllerState.InstantUsage[heater.Heater.UsageUnit] -= heater.Heater.UsagePerHour;
                    }
                }
            }
        }
    }
}
