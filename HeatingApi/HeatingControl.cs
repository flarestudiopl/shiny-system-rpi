﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Commons;
using Commons.Localization;
using HeatingControl.Application;
using HeatingControl.Application.Loops;
using HeatingControl.Application.Commands;
using HeatingControl.Models;
using Microsoft.Extensions.Hosting;
using Storage.StorageDatabase;
using Microsoft.AspNetCore.Hosting;
using HeatingControl.Application.DataAccess.Building;

namespace HeatingApi
{
    public interface IHeatingControl : IDisposable
    {
        void Start();
        void SetControlEnabled(bool controlEnabled);
        ControllerState State { get; }
    }

    public class HeatingControl : IHeatingControl, IHostedService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly IApplicationLifetime _appLifetime;
        private readonly IBuildingProvider _buildingProvider;
        private readonly ICommandExecutor<DisableAllOutputsCommand> _disableAllOutputsCommandExecutor;
        private readonly ITemperatureReadingLoop _temperatureReadingLoop;
        private readonly IScheduleDeterminationLoop _scheduleDeterminationLoop;
        private readonly IOutputStateProcessingLoop _outputStateProcessingLoop;
        private readonly IDigitalInputReadingLoop _digitalInputReadingLoop;

        public ControllerState State { get; }

        public HeatingControl(IApplicationLifetime appLifetime,
                              IMigrator migrator,
                              IBuildingProvider buildingProvider,
                              IControllerStateBuilder controllerStateBuilder,
                              ICommandExecutor<DisableAllOutputsCommand> disableAllOutputsCommandExecutor,
                              ITemperatureReadingLoop temperatureReadingLoop,
                              IScheduleDeterminationLoop scheduleDeterminationLoop,
                              IOutputStateProcessingLoop outputStateProcessingLoop,
                              IDigitalInputReadingLoop digitalInputReadingLoop)
        {
            migrator.Run();

            _appLifetime = appLifetime;
            _buildingProvider = buildingProvider;
            _disableAllOutputsCommandExecutor = disableAllOutputsCommandExecutor;
            _temperatureReadingLoop = temperatureReadingLoop;
            _scheduleDeterminationLoop = scheduleDeterminationLoop;
            _outputStateProcessingLoop = outputStateProcessingLoop;
            _digitalInputReadingLoop = digitalInputReadingLoop;

            var model = _buildingProvider.ProvideDefault();
            State = controllerStateBuilder.Build(model);
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _appLifetime.ApplicationStopping.Register(Dispose);

            Logger.Info(Localization.NotificationMessage.DisablingAllOutputs);

            _disableAllOutputsCommandExecutor.Execute(null, new CommandContext { ControllerState = State });

            Logger.Info(Localization.NotificationMessage.StartingControlLoops);

            _temperatureReadingLoop.Start(State, _cancellationTokenSource.Token);
            _scheduleDeterminationLoop.Start(State, _cancellationTokenSource.Token);
            _outputStateProcessingLoop.Start(State, _cancellationTokenSource.Token);
            _digitalInputReadingLoop.Start(State, _cancellationTokenSource.Token);

            State.ControlEnabled = true;
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

            State.ControlEnabled = false;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => Start(), cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void SetControlEnabled(bool controlEnabled)
        {
            if (controlEnabled && !State.ControlEnabled)
            {
                Start();
            }

            if (!controlEnabled && State.ControlEnabled)
            {
                Dispose();
            }
        }
    }
}
