using System.Linq;
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
                     DescriptorsEqual(existingHeater.Heater.DigitalOutput.OutputDescriptor, command.PowerOutputDescriptor)))
                {
                    outputToDisable = new DigitalOutput
                    {
                        ProtocolName = existingHeater.Heater.DigitalOutput.ProtocolName,
                        OutputDescriptor = existingHeater.Heater.DigitalOutput.OutputDescriptor
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
                                    .TrySetState(outputToDisable.OutputDescriptor, false);
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

            var heaterWithSameOutput = context.ControllerState.Model.Heaters.FirstOrDefault(x => x.DigitalOutput.ProtocolName == command.PowerOutputProtocolName &&
                                                                                                 DescriptorsEqual(x.DigitalOutput.OutputDescriptor, command.PowerOutputDescriptor));

            if (heaterWithSameOutput != null &&
                (!command.HeaterId.HasValue || command.HeaterId.Value != heaterWithSameOutput.HeaterId))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.PowerOutputParametersAlreadyAssigned.FormatWith(command.PowerOutputDescriptor));
            }

            return null;
        }

        private static bool DescriptorsEqual(string serializedDescriptor, object jObject)
        {
            var descriptor = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(serializedDescriptor);

            var check = Newtonsoft.Json.Linq.JObject.DeepEquals(descriptor, (Newtonsoft.Json.Linq.JObject)jObject);

            return check;
        }
    }
}
