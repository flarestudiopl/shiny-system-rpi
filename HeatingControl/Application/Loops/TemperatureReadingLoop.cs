using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Commons;
using Commons.Extensions;
using Commons.Localization;
using HardwareAccess.Devices;
using HardwareAccess.Devices.TemperatureInputs;
using HeatingControl.Models;

namespace HeatingControl.Application.Loops
{
    public interface ITemperatureReadingLoop
    {
        void Start(ControllerState controllerState, CancellationToken cancellationToken);
    }

    public class TemperatureReadingLoop : ITemperatureReadingLoop
    {
        private const int TempAvgQueueLength = 5;
        private const int TempHistoryMinutesModule = 2;
        private int _tempHistoryQueueLength = 200; //60 * 24 / TempHistoryMinutesModule;

        private readonly ITemperatureInputProvider _temperatureInputProvider;

        public TemperatureReadingLoop(ITemperatureInputProvider temperatureInputProvider)
        {
            _temperatureInputProvider = temperatureInputProvider;
        }

        public void Start(ControllerState controllerState, CancellationToken cancellationToken)
        {
            Loop.Start("Temperature processing",
                       controllerState.Model.ControlLoopIntervalSecondsMilliseconds,
                       () => ProcessReads(controllerState),
                       cancellationToken);
        }

        private void ProcessReads(ControllerState controllerState)
        {
            Parallel.ForEach(controllerState.TemperatureSensorIdToState,
                             sensor =>
                             {
                                 var sensorRead = _temperatureInputProvider.Provide(sensor.Value.TemperatureSensor.ProtocolName)
                                                                           .GetValue(sensor.Value.TemperatureSensor.InputDescriptor);

                                 sensorRead.Wait();

                                 if (sensorRead.Result.CrcOk)
                                 {
                                     var temperatureSensorState = sensor.Value;

                                     ProcessCurrentRead(temperatureSensorState, sensorRead);
                                     ProcessHistory(temperatureSensorState);
                                 }
                                 else
                                 {
                                     Logger.Warning(Localization.NotificationMessage.SensorCrcError.FormatWith(sensor.Key));
                                 }
                             });
        }

        private static void ProcessCurrentRead(TemperatureSensorState temperatureSensorState, Task<TemperatureSensorData> sensorRead)
        {
            temperatureSensorState.Readouts.Enqueue(sensorRead.Result.Value);
            temperatureSensorState.AverageTemperature = temperatureSensorState.Readouts.Average(x => x);
            temperatureSensorState.LastRead = DateTime.UtcNow;

            if (temperatureSensorState.Readouts.Count > TempAvgQueueLength)
            {
                temperatureSensorState.Readouts.Dequeue();
            }
        }

        private void ProcessHistory(TemperatureSensorState temperatureSensorState)
        {
            var now = DateTime.UtcNow;

            if (now.Minute % TempHistoryMinutesModule == 0 && temperatureSensorState.HistoricalReads.LastOrDefault()?.Item1.Minute != now.Minute)
            {
                temperatureSensorState.HistoricalReads.Enqueue(Tuple.Create(now, Math.Round(temperatureSensorState.AverageTemperature, 1)));
            }

            if (temperatureSensorState.HistoricalReads.Count > _tempHistoryQueueLength)
            {
                temperatureSensorState.HistoricalReads.Dequeue();
            }
        }
    }
}
