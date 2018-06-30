using System;
using System.Threading;
using System.Threading.Tasks;
using Commons;
using HeatingControl.Application;
using HeatingControl.Application.Loops;
using HeatingControl.Application.Commands;
using HeatingControl.Models;
using Microsoft.Extensions.Hosting;
using Storage.BuildingModel;

namespace HeatingApi
{
    public interface IHeatingControl : IDisposable
    {
        void Start();
        ControllerState State { get; }
        bool ControlEnabled { get; set; }
    }

    public class HeatingControl : IHeatingControl, IHostedService
    {
        private CancellationTokenSource _cancellationTokenSource;

        private readonly IDisableAllOutputsExecutor _disableAllOutputsExecutor;
        private readonly ITemperatureReadingLoop _temperatureReadingLoop;
        private readonly IScheduleDeterminationLoop _scheduleDeterminationLoop;
        private readonly IOutputStateProcessingLoop _outputStateProcessingLoop;

        public ControllerState State { get; }

        private bool _controlEnabled = true;
        public bool ControlEnabled
        {
            get => _controlEnabled;
            set
            {
                if (value && !_controlEnabled)
                {
                    Start();
                }

                if (!value && _controlEnabled)
                {
                    Dispose();
                }

                _controlEnabled = value;
            }
        }

        public HeatingControl(IBuildingModelProvider buildingModelProvider,
                              IControllerStateBuilder controllerStateBuilder,
                              IDisableAllOutputsExecutor disableAllOutputsExecutor,
                              ITemperatureReadingLoop temperatureReadingLoop,
                              IScheduleDeterminationLoop scheduleDeterminationLoop,
                              IOutputStateProcessingLoop outputStateProcessingLoop)
        {
            _disableAllOutputsExecutor = disableAllOutputsExecutor;
            _temperatureReadingLoop = temperatureReadingLoop;
            _scheduleDeterminationLoop = scheduleDeterminationLoop;
            _outputStateProcessingLoop = outputStateProcessingLoop;

            var model = buildingModelProvider.Provide();
            State = controllerStateBuilder.Build(model);
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Logger.Info("Disabling all outputs...");

            _disableAllOutputsExecutor.Execute(State);

            Logger.Info("Starting control loops...");

            // TODO - different intervals

            _temperatureReadingLoop.Start(State, _cancellationTokenSource.Token);
            _scheduleDeterminationLoop.Start(State.Model.ControlLoopIntervalSecondsMilliseconds, State, _cancellationTokenSource.Token); //TODO
            _outputStateProcessingLoop.Start(State.Model.ControlLoopIntervalSecondsMilliseconds, State, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            using (_cancellationTokenSource)
            {
                Logger.Info("Sending cancellation to control loops...");

                _cancellationTokenSource.Cancel();

                Logger.Info("Disabling all outputs...");

                _disableAllOutputsExecutor.Execute(State);
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
