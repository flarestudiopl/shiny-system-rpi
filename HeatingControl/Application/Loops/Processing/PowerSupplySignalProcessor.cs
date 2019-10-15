using Commons.Extensions;
using Domain;
using HeatingControl.Application.Commands;
using HeatingControl.Models;
using System;
using System.Collections.Generic;

namespace HeatingControl.Application.Loops.Processing
{
    public interface IPowerSupplySignalProcessor
    {
        void Process(ControllerState controllerState);
    }

    public class PowerSupplySignalProcessor : IPowerSupplySignalProcessor
    {
        private const double MINUTES_TO_SHUTDOWN_IN_BATTERY_MODE = 5.0d;

        private readonly ICommandExecutor<PowerOffCommand> _powerOffCommandExecutor;

        public PowerSupplySignalProcessor(ICommandExecutor<PowerOffCommand> powerOffCommandExecutor)
        {
            _powerOffCommandExecutor = powerOffCommandExecutor;
        }

        public void Process(ControllerState controllerState)
        {
            var batteryMode = controllerState.DigitalInputFunctionToState.GetValueOrDefault(DigitalInputFunction.BatteryMode)?.State ?? false;
            var beginShutdown = controllerState.DigitalInputFunctionToState.GetValueOrDefault(DigitalInputFunction.BeginShutdown)?.State ?? false;

            if (!batteryMode)
            {
                controllerState.ScheduledShutdownUtcTime = null;
            }
            else if(!controllerState.ScheduledShutdownUtcTime.HasValue)
            {
                controllerState.ScheduledShutdownUtcTime = DateTime.UtcNow.AddMinutes(MINUTES_TO_SHUTDOWN_IN_BATTERY_MODE);
            }

            if ((controllerState.ScheduledShutdownUtcTime <= DateTime.UtcNow || beginShutdown) && !controllerState.ShutdownRequested)
            {
                controllerState.ShutdownRequested = true;

                _powerOffCommandExecutor.Execute(new PowerOffCommand(), null);
            }
        }
    }
}
