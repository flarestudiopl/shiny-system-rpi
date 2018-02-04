using Commons;
using HardwareAccess.Devices;
using HeatingControl.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeatingControl.Application
{
    public interface ITemperatureReadingLoop
    {
        void Start(int intervalMilliseconds, ControllerState controllerState, CancellationToken cancellationToken);
    }

    public class TemperatureReadingLoop : ITemperatureReadingLoop
    {
        private const int TEMP_AVG_QUEUE_LENGTH = 5;

        private readonly ITemperatureSensor _temperatureSensor;

        public TemperatureReadingLoop(ITemperatureSensor temperatureSensor)
        {
            _temperatureSensor = temperatureSensor;
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

                        ProcessReads(controllerState);

                        Logger.Trace("Temperature processing loop took {0} ms.", new object[] { stopwatch.ElapsedMilliseconds });

                        Thread.Sleep(intervalMilliseconds);
                    }
                },
                cancellationToken);
        }

        private void ProcessReads(ControllerState controllerState)
        {
            foreach (var zone in controllerState.DeviceIdToTemperatureData)
            {
                var sensorRead = _temperatureSensor.Read(zone.Key);
                sensorRead.Wait();

                if (sensorRead.Result.CrcOk)
                {
                    var temperatureData = zone.Value;

                    temperatureData.Readouts.Enqueue(sensorRead.Result.Value);
                    temperatureData.AverageTemperature = temperatureData.Readouts.Average(x => x);
                    temperatureData.LastRead = DateTime.Now;

                    if (temperatureData.Readouts.Count > TEMP_AVG_QUEUE_LENGTH)
                    {
                        temperatureData.Readouts.Dequeue();
                    }
                }
                else
                {
                    Logger.Warning($"Sensor {zone.Key} CRC error. Skipping readout.");
                }
            }
        }
    }
}
