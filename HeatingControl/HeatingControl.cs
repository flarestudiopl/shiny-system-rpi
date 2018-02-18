using HeatingControl.Application;
using HeatingControl.Models;
using Storage.BuildingModel;
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

        private readonly IBuildingModelProvider _buildingModelProvider;
        private readonly IControllerStateBuilder _controllerStateBuilder;
        private readonly ITemperatureReadingLoop _temperatureReadingLoop;
        private readonly IOutputStateProcessingLoop _outputStateProcessingLoop;

        public ControllerState State { get; private set; }

        public HeatingControl(IBuildingModelProvider buildingModelProvider,
                              IControllerStateBuilder controllerStateBuilder,
                              ITemperatureReadingLoop temperatureReadingLoop,
                              IOutputStateProcessingLoop outputStateProcessingLoop)
        {
            _buildingModelProvider = buildingModelProvider;
            _controllerStateBuilder = controllerStateBuilder;
            _temperatureReadingLoop = temperatureReadingLoop;
            _outputStateProcessingLoop = outputStateProcessingLoop;
        }

        public void Start()
        {
            var buildingModel = _buildingModelProvider.Provide();
            State = _controllerStateBuilder.Build(buildingModel);

            _temperatureReadingLoop.Start(buildingModel.ControlLoopIntervalSecondsMilliseconds, State, _cancellationTokenSource.Token);
            _outputStateProcessingLoop.Start(buildingModel.ControlLoopIntervalSecondsMilliseconds, State, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
