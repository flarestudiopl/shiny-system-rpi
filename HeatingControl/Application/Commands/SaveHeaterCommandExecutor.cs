﻿using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Domain;
using HardwareAccess.Devices;
using HeatingControl.Application.DataAccess.Heater;
using HeatingControl.Models;

namespace HeatingControl.Application.Commands
{
    public class SaveHeaterCommand : HeaterSaverInput { }

    public class SaveHeaterCommandExecutor : ICommandExecutor<SaveHeaterCommand>
    {
        private readonly IHeaterSaver _heaterSaver;
        private readonly IPowerOutputProvider _powerOutputProvider;

        public SaveHeaterCommandExecutor(IHeaterSaver heaterSaver,
                                         IPowerOutputProvider powerOutputProvider)
        {
            _heaterSaver = heaterSaver;
            _powerOutputProvider = powerOutputProvider;
        }

        public CommandResult Execute(SaveHeaterCommand command, CommandContext context)
        {
            var validationResult = Validate(command, context);

            if (validationResult != null)
            {
                return validationResult;
            }

            DigitalOutput outputToDisable = null;

            if (command.HeaterId.HasValue)
            {
                var existingHeater = context.ControllerState.HeaterIdToState.GetValueOrDefault(command.HeaterId.Value);

                if (existingHeater != null &&
                    (existingHeater.Heater.DigitalOutput.ProtocolName != command.PowerOutputProtocolName ||
                     existingHeater.Heater.DigitalOutput.DeviceId != command.PowerOutputDeviceId ||
                     existingHeater.Heater.DigitalOutput.OutputChannel != command.PowerOutputChannel))
                {
                    outputToDisable = new DigitalOutput
                    {
                        ProtocolName = existingHeater.Heater.DigitalOutput.ProtocolName,
                        DeviceId = existingHeater.Heater.DigitalOutput.DeviceId,
                        OutputChannel = existingHeater.Heater.DigitalOutput.OutputChannel
                    };
                }
            }

            var heater = _heaterSaver.Save(command, context.ControllerState.Model);
            var heaterState = context.ControllerState.HeaterIdToState.GetValueOrDefault(heater.HeaterId);

            if (heaterState == null)
            {
                context.ControllerState.HeaterIdToState.Add(heater.HeaterId, new HeaterState
                {
                    Heater = heater
                });
            }
            else
            {
                heaterState.Heater = heater;
            }

            if (outputToDisable != null)
            {
                _powerOutputProvider.Provide(outputToDisable.ProtocolName)
                                    .SetState(outputToDisable.DeviceId, outputToDisable.OutputChannel, false);
            }

            return CommandResult.Empty;
        }

        private static CommandResult Validate(SaveHeaterCommand command, CommandContext context)
        {
            if (command.Name.IsNullOrEmpty())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.NameCantBeEmpty);
            }

            if (command.UsagePerHour < 0m)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UsageCantBeNegative);
            }

            if (command.MinimumStateChangeIntervalSeconds < 0)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.MinimumStateChangeIntervalCantBeNegative);
            }

            var heaterWithSameOutput = context.ControllerState.Model.Heaters.FirstOrDefault(x => x.DigitalOutput.DeviceId == command.PowerOutputDeviceId &&
                                                                                                 x.DigitalOutput.OutputChannel == command.PowerOutputChannel &&
                                                                                                 x.DigitalOutput.ProtocolName == command.PowerOutputProtocolName);

            if (heaterWithSameOutput != null &&
                (!command.HeaterId.HasValue || command.HeaterId.Value != heaterWithSameOutput.HeaterId))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.PowerOutputParametersAlreadyAssigned.FormatWith(command.PowerOutputDeviceId, command.PowerOutputChannel));
            }

            return null;
        }
    }
}
