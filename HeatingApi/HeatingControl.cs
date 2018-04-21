using System;
using System.Threading;
using System.Threading.Tasks;
using Commons;
using HeatingControl.Application;
using HeatingControl.Application.Loops;
using HeatingControl.Domain;
using HeatingControl.Models;
using Microsoft.Extensions.Hosting;
using Storage.BuildingModel;

namespace HeatingApi
{
    public interface IHeatingControl : IDisposable
    {
        void Start();
        Building Model { get; }
        ControllerState State { get; }
    }

    public class HeatingControl : IHeatingControl, IHostedService
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly IBuildingModelProvider _buildingModelProvider;
        private readonly IControllerStateBuilder _controllerStateBuilder;
        private readonly ITemperatureReadingLoop _temperatureReadingLoop;
        private readonly IScheduleDeterminationLoop _scheduleDeterminationLoop;
        private readonly IOutputStateProcessingLoop _outputStateProcessingLoop;

        public Building Model { get; private set; }
        public ControllerState State { get; private set; }

        public HeatingControl(IBuildingModelProvider buildingModelProvider,
                              IControllerStateBuilder controllerStateBuilder,
                              ITemperatureReadingLoop temperatureReadingLoop,
                              IScheduleDeterminationLoop scheduleDeterminationLoop,
                              IOutputStateProcessingLoop outputStateProcessingLoop)
        {
            _buildingModelProvider = buildingModelProvider;
            _controllerStateBuilder = controllerStateBuilder;
            _temperatureReadingLoop = temperatureReadingLoop;
            _scheduleDeterminationLoop = scheduleDeterminationLoop;
            _outputStateProcessingLoop = outputStateProcessingLoop;
        }

        public void Start()
        {
            Model = _buildingModelProvider.Provide();
            State = _controllerStateBuilder.Build(Model);

            Logger.Info("Starting control loops...");

            // TODO - different intervals

            _temperatureReadingLoop.Start(Model.ControlLoopIntervalSecondsMilliseconds, State, _cancellationTokenSource.Token);
            _scheduleDeterminationLoop.Start(Model.ControlLoopIntervalSecondsMilliseconds, State, _cancellationTokenSource.Token);
            _outputStateProcessingLoop.Start(Model.ControlLoopIntervalSecondsMilliseconds, State, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            using (_cancellationTokenSource)
            {
                Logger.Info("Sending cancellation to control loops...");

                _cancellationTokenSource.Cancel();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => Start(), cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => Dispose(), cancellationToken);
        }
    }
}
