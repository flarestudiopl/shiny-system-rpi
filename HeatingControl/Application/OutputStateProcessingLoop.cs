using Commons;
using HeatingControl.Domain;
using HeatingControl.Models;
using System;
using System.Diagnostics;
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
                ScheduleItem scheduleItem = null; // TODO

                switch (temperatureZone.CurrentControlType)
                {
                    case ControlType.None:
                    case ControlType.ManualOnOff:
                        break;
                    case ControlType.ScheduleOnOff:
                        outputState = scheduleItem != null;
                        break;
                    case ControlType.ScheduleTemperatureControl:
                        temperatureZone.SetPoint = scheduleItem.SetPoint.Value;
                        outputState = ProcessHisteresis(controllerState.DeviceIdToTemperatureData[temperatureZone.TemperatureZone.TemperatureSensorDeviceId], temperatureZone.SetPoint);
                        break;
                    case ControlType.ManualTemperatureControl:
                        outputState = ProcessHisteresis(controllerState.DeviceIdToTemperatureData[temperatureZone.TemperatureZone.TemperatureSensorDeviceId], temperatureZone.SetPoint);
                        break;
                }
            }
        }

        private bool ProcessHisteresis(TemperatureData temperatureData, float value)
        {
            throw new NotImplementedException();
        }

        private void ProcessHeaters(ControllerState controllerState)
        {
            throw new NotImplementedException();
        }

        private void ProcessPowerZones(ControllerState controllerState)
        {
            throw new NotImplementedException();
        }

        private void WriteOutputs(ControllerState controllerState)
        {
            throw new NotImplementedException();
        }

    }
}
