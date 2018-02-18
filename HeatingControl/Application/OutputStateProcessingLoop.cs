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
            foreach (var zone in controllerState.TemperatureZoneNameToState)
            {
                var temperatureZone = zone.Value;
                var outputState = temperatureZone.EnableOutputs;
                var scheduleItem = TryGetScheduleItem(zone.Value);

                switch (temperatureZone.CurrentControlType)
                {
                    case ControlType.ScheduleOnOff:
                        outputState = scheduleItem != null;
                        break;
                    case ControlType.ScheduleTemperatureControl:
                        temperatureZone.SetPoint = scheduleItem?.SetPoint.Value ?? temperatureZone.TemperatureZone.DefaultSetPoint;
                        outputState = ProcessTemperatureBasedOutput(controllerState, temperatureZone, outputState);
                        break;
                    case ControlType.ManualTemperatureControl:
                        outputState = ProcessTemperatureBasedOutput(controllerState, temperatureZone, outputState);
                        break;
                    default:
                        break;
                }

                temperatureZone.EnableOutputs = outputState;
            }
        }

        private bool ProcessTemperatureBasedOutput(ControllerState controllerState, TemperatureZoneState temperatureZone, bool outputState)
        {
            var temperatureData = controllerState.DeviceIdToTemperatureData[temperatureZone.TemperatureZone.TemperatureSensorDeviceId];

            if (DateTime.Now - temperatureData.LastRead < TimeSpan.FromMinutes(5))
            {
                outputState = _hysteresisProcessor.Process(temperatureData.AverageTemperature,
                                                           outputState,
                                                           temperatureZone.SetPoint,
                                                           temperatureZone.TemperatureZone.Hysteresis);
            }
            else
            {
                outputState = false;
                Logger.Trace("Temperature value for zone {0} is too old. Proactive power cutoff.", new[] { temperatureZone.TemperatureZone.Name });
            }

            return outputState;
        }

        private ScheduleItem TryGetScheduleItem(TemperatureZoneState zoneState)
        {
            var schedule = zoneState.TemperatureZone.Schedule;
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
            foreach (var zone in controllerState.TemperatureZoneNameToState.Values)
            {
                foreach (var heater in zone.TemperatureZone.Heaters)
                {
                    controllerState.HeaterNameToState[heater.Name].OutputState = zone.EnableOutputs;
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
