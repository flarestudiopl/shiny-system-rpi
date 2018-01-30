using System;
using System.Threading;

namespace HeatingControl
{
    public interface IHeatingControl : IDisposable
    {
        void Start();
        ControllerState State { get; } // TODO keep state other way
    }

    public class HeatingControl : IHeatingControl
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ITemperatureReadingLoop _temperatureReadingLoop;

        public ControllerState State { get; }

        public HeatingControl(IBuildingModelProvider buildingModelProvider,
                              IControllerStateBuilder controllerStateBuilder,
                              ITemperatureReadingLoop temperatureReadingLoop)
        {
            var buildingModel = buildingModelProvider.Provide();
            State = controllerStateBuilder.Build(buildingModel);
            _temperatureReadingLoop = temperatureReadingLoop;
        }

        public void Start()
        {
            _temperatureReadingLoop.Start(1000, State, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
