using Commons;
using HardwareAccess.Devices;
using HeatingControl.Domain;
using HeatingControl.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeatingControl.Application
{
    public interface IOutputStateProcessingLoop
    {
        void Start(int intervalMilliseconds, ControllerState controllerState, CancellationToken cancellationToken);
    }

    public class OutputStateProcessingLoop : IOutputStateProcessingLoop
    {
        private readonly IPowerOutput _powerOutput;
        private readonly IHysteresisProcessor _hysteresisProcessor;

        public OutputStateProcessingLoop(IPowerOutput powerOutput,
                                         IHysteresisProcessor hysteresisProcessor)
        {
            _powerOutput = powerOutput;
            _hysteresisProcessor = hysteresisProcessor;
        }

        public void Start(int intervalMilliseconds, ControllerState controllerState, CancellationToken cancellationToken)
        {
            Task.Run(
                () =>
                {
                    while (true)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var stopwatch = Stopwatch.StartNew();

                        ProcessTemperatureZones(controllerState);
                        ProcessHeaters(controllerState);
                        ProcessPowerZones(controllerState);

                        WriteOutputs(controllerState);

                        Logger.Trace("Output state processing loop took {0} ms.", new object[] { stopwatch.ElapsedMilliseconds });

                        Thread.Sleep(intervalMilliseconds);
                    }
                },
                cancellationToken);
        }

        private void ProcessTemperatureZones(ControllerState controllerState)
        {
            foreach (var zoneState in controllerState.ZoneNameToState.Values)
            {
                if (zoneState.Zone.TemperatureControlledZone != null)
                {
                    zoneState.EnableOutputs = ProcessTemperatureBasedOutput(controllerState, zoneState);
                }
                else
                {
                    zoneState.EnableOutputs = ProcessOnOffBasedOutput(controllerState, zoneState);
                }
            }
        }

        private bool ProcessTemperatureBasedOutput(ControllerState controllerState, ZoneState zoneState)
        {
            var temperatureSensorDeviceId = controllerState.TemperatureSensorNameToDeviceId[zoneState.Zone.TemperatureControlledZone.TemperatureSensorName];
            var temperatureData = controllerState.DeviceIdToTemperatureData[temperatureSensorDeviceId];

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
                        var scheduleItem = TryGetScheduleItem(zoneState);
                        setPoint = scheduleItem != null ? scheduleItem.SetPoint.Value : zoneState.Zone.TemperatureControlledZone.ScheduleDefaultSetPoint;
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
                Logger.Trace("Temperature value for zone {0} is too old. Proactive power cutoff.", new[] { zoneState.Zone.Name });
            }

            return outputState;
        }

        private bool ProcessOnOffBasedOutput(ControllerState controllerState, ZoneState zoneState)
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
                    outputState = TryGetScheduleItem(zoneState) != null;
                    break;
            }

            return outputState;
        }

        private ScheduleItem TryGetScheduleItem(ZoneState zoneState)
        {
            var schedule = zoneState.Zone.Schedule;
            var now = DateTime.Now;

            if (schedule != null && schedule.Any())
            {
                return schedule.FirstOrDefault(x => x.DayOfWeek == now.DayOfWeek &&
                                               x.BeginTime.TimeOfDay > now.TimeOfDay &&
                                               x.EndTime.TimeOfDay <= now.TimeOfDay);
            }

            return null;
        }

        private void ProcessHeaters(ControllerState controllerState)
        {
            foreach (var zone in controllerState.ZoneNameToState.Values)
            {
                foreach (var heater in zone.Zone.HeatersNames)
                {
                    controllerState.HeaterNameToState[heater].OutputState = zone.EnableOutputs;
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

            foreach (var heater in controllerState.HeaterNameToState.Values)
            {
                var powerOutput = heater.Heater.PowerOutput;

                if ((now - heater.LastStateChange).TotalSeconds > heater.Heater.MinimumStateChangeIntervalSeconds &&
                    heater.OutputState != _powerOutput.GetState(powerOutput.PowerOutputDeviceId, powerOutput.PowerOutputChannel))
                {
                    _powerOutput.SetState(powerOutput.PowerOutputDeviceId, powerOutput.PowerOutputChannel, heater.OutputState);
                    heater.LastStateChange = now;
                }
            }
        }
    }
}
