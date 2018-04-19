using HeatingControl.Application;
using HeatingControl.Models;
using Storage.BuildingModel;
using System;
using System.Threading;
using HeatingControl.Domain;

namespace HeatingControl
{
    public interface IHeatingControl : IDisposable
    {
        void Start();
        Building Model { get; }
        ControllerState State { get; }
    }

    public class HeatingControl : IHeatingControl
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly IBuildingModelProvider _buildingModelProvider;
        private readonly IControllerStateBuilder _controllerStateBuilder;
        private readonly ITemperatureReadingLoop _temperatureReadingLoop;
        private readonly IOutputStateProcessingLoop _outputStateProcessingLoop;

        public Building Model { get; private set; }
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
            Model = _buildingModelProvider.Provide();
            State = _controllerStateBuilder.Build(Model);

            _temperatureReadingLoop.Start(Model.ControlLoopIntervalSecondsMilliseconds, State, _cancellationTokenSource.Token);
            _outputStateProcessingLoop.Start(Model.ControlLoopIntervalSecondsMilliseconds, State, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
