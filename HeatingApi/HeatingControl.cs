using System;
using System.Threading;
using System.Threading.Tasks;
using Commons;
using Commons.Localization;
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

        private readonly ICommandExecutor<DisableAllOutputsCommand> _disableAllOutputsCommandExecutor;
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
                              ICommandExecutor<DisableAllOutputsCommand> disableAllOutputsCommandExecutor,
                              ITemperatureReadingLoop temperatureReadingLoop,
                              IScheduleDeterminationLoop scheduleDeterminationLoop,
                              IOutputStateProcessingLoop outputStateProcessingLoop)
        {
            _disableAllOutputsCommandExecutor = disableAllOutputsCommandExecutor;
            _temperatureReadingLoop = temperatureReadingLoop;
            _scheduleDeterminationLoop = scheduleDeterminationLoop;
            _outputStateProcessingLoop = outputStateProcessingLoop;

            var model = buildingModelProvider.Provide();
            State = controllerStateBuilder.Build(model);
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Logger.Info(Localization.NotificationMessage.DisablingAllOutputs);

            _disableAllOutputsCommandExecutor.Execute(null, new CommandContext { ControllerState = State });

            Logger.Info(Localization.NotificationMessage.StartingControlLoops);

            _temperatureReadingLoop.Start(State, _cancellationTokenSource.Token);
            _scheduleDeterminationLoop.Start(State, _cancellationTokenSource.Token);
            _outputStateProcessingLoop.Start(State, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            using (_cancellationTokenSource)
            {
                Logger.Info(Localization.NotificationMessage.SendingCancellationToControlLoops);

                _cancellationTokenSource.Cancel();

                Logger.Info(Localization.NotificationMessage.DisablingAllOutputs);

                _disableAllOutputsCommandExecutor.Execute(null, new CommandContext { ControllerState = State });
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
