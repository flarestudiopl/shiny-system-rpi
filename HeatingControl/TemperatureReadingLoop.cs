using Commons;
using HardwareAccess.Devices;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeatingControl
{
    public interface ITemperatureReadingLoop
    {
        void Start(int intervalMilliseconds, CancellationToken cancellationToken);
    }

    public class TemperatureReadingLoop : ITemperatureReadingLoop
    {
        private const int TEMP_AVG_QUEUE_LENGTH = 5;

        private readonly ControllerState _controllerState;
        private readonly TemperatureSensor _temperatureSensor;

        public TemperatureReadingLoop(ControllerState controllerState,
                                      TemperatureSensor temperatureSensor)
        {
            _controllerState = controllerState;
            _temperatureSensor = temperatureSensor;
        }

        public void Start(int intervalMilliseconds, CancellationToken cancellationToken)
        {
            Task.Run(
                () =>
                {
                    while (true)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var stopwatch = Stopwatch.StartNew();

                        ProcessReads();

                        Logger.Trace("Temperature processing loop took {0} ms.", new object[] { stopwatch.ElapsedMilliseconds });

                        Thread.Sleep(intervalMilliseconds);
                    }
                },
                cancellationToken);
        }

        private async void ProcessReads()
        {
            foreach (var zone in _controllerState.DeviceIdToTemperatureData)
            {
                var currentReadout = await _temperatureSensor.Read(zone.Key);

                if (currentReadout.CrcOk)
                {
                    var temperatureData = zone.Value;

                    temperatureData.Readouts.Enqueue(currentReadout);
                    temperatureData.AverageTemperature = temperatureData.Readouts.Average(x => x.Value);
                    temperatureData.LastRead = DateTime.Now;

                    if (temperatureData.Readouts.Count > TEMP_AVG_QUEUE_LENGTH)
                    {
                        temperatureData.Readouts.Dequeue();
                    }
                }
                else
                {
                    Logger.Warning($"Sensor {zone} CRC error. Skipping readout.");
                }
            }
        }
    }
}
