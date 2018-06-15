using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Commons;
using HardwareAccess.Devices;
using HeatingControl.Models;

namespace HeatingControl.Application.Loops
{
    public interface ITemperatureReadingLoop
    {
        void Start(int intervalMilliseconds, ControllerState controllerState, CancellationToken cancellationToken);
    }

    public class TemperatureReadingLoop : ITemperatureReadingLoop
    {
        private const int TempAvgQueueLength = 5;
        private const int TempHistoryMinutesModule = 2;

        private int _tempHistoryQueueLength = 200;//60 * 24 / TempHistoryMinutesModule;

        private readonly ITemperatureSensor _temperatureSensor;

        public TemperatureReadingLoop(ITemperatureSensor temperatureSensor)
        {
            _temperatureSensor = temperatureSensor;
        }

        public void Start(int intervalMilliseconds, ControllerState controllerState, CancellationToken cancellationToken)
        {
            Loop.Start("Temperature processing",
                       intervalMilliseconds,
                       () => ProcessReads(controllerState),
                       cancellationToken);
        }

        private void ProcessReads(ControllerState controllerState)
        {
            Parallel.ForEach(controllerState.TemperatureDeviceIdToTemperatureData,
                             zone =>
                             {
                                 var sensorRead = _temperatureSensor.Read(zone.Key);
                                 sensorRead.Wait();

                                 if (sensorRead.Result.CrcOk)
                                 {
                                     var temperatureData = zone.Value;

                                     ProcessCurrentRead(temperatureData, sensorRead);
                                     ProcessHistory(temperatureData);
                                 }
                                 else
                                 {
                                     Logger.Warning($"Sensor {zone.Key} CRC error. Skipping readout.");
                                 }
                             });
        }

        private static void ProcessCurrentRead(TemperatureData temperatureData, Task<TemperatureSensorData> sensorRead)
        {
            temperatureData.Readouts.Enqueue(sensorRead.Result.Value);
            temperatureData.AverageTemperature = temperatureData.Readouts.Average(x => x);
            temperatureData.LastRead = DateTime.Now;

            if (temperatureData.Readouts.Count > TempAvgQueueLength)
            {
                temperatureData.Readouts.Dequeue();
            }
        }

        private void ProcessHistory(TemperatureData temperatureData)
        {
            var now = DateTime.Now;

            if (now.Minute % TempHistoryMinutesModule == 0 && temperatureData.HistoricalReads.LastOrDefault()?.Item1.Minute != now.Minute)
            {
                temperatureData.HistoricalReads.Enqueue(Tuple.Create(now, temperatureData.AverageTemperature));
            }

            if (temperatureData.HistoricalReads.Count > _tempHistoryQueueLength)
            {
                temperatureData.HistoricalReads.Dequeue();
            }
        }
    }
}
