using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using HeatingControl.Application.DataAccess.TemperatureSensor;

namespace HeatingControl.Application.Commands
{
    public class SaveTemperatureSensorCommand : TemperatureSensorSaverInput { }

    public class SaveTemperatureSensorCommandExecutor : ICommandExecutor<SaveTemperatureSensorCommand>
    {
        private readonly ITemperatureSensorSaver _temperatureSensorSaver;

        public SaveTemperatureSensorCommandExecutor(ITemperatureSensorSaver temperatureSensorSaver)
        {
            _temperatureSensorSaver = temperatureSensorSaver;
        }

        public CommandResult Execute(SaveTemperatureSensorCommand command, CommandContext context)
        {
            if (command.Name.IsNullOrEmpty())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.NameCantBeEmpty);
            }

            if (command.ProtocolName.IsNullOrEmpty())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.ProtocolNameCannotBeEmpty);
            }

            var existingTemperatureSensor = context.ControllerState.Model.TemperatureSensors?.FirstOrDefault(x => x.ProtocolName == command.ProtocolName &&
                                                                                                                  DescriptorsEqual(x.InputDescriptor, command.InputDescriptor));

            if (existingTemperatureSensor != null)
            {
                if (!command.SensorId.HasValue || command.SensorId.Value != existingTemperatureSensor.TemperatureSensorId)
                {
                    return CommandResult.WithValidationError(Localization.ValidationMessage.DeviceIdAlreadyInUse);
                }
            }

            var temperatureSensor = _temperatureSensorSaver.Save(command, context.ControllerState.Model);

            if (context.ControllerState.TemperatureSensorIdToState.ContainsKey(temperatureSensor.TemperatureSensorId))
            {
                context.ControllerState.TemperatureSensorIdToState[temperatureSensor.TemperatureSensorId].TemperatureSensor = temperatureSensor;
            }
            else
            {
                context.ControllerState.TemperatureSensorIdToState.Add(temperatureSensor.TemperatureSensorId, new Models.TemperatureSensorState { TemperatureSensor = temperatureSensor });
            }

            return CommandResult.Empty;
        }

        private static bool DescriptorsEqual(string serializedDescriptor, object jObject)
        {
            var descriptor = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(serializedDescriptor);

            var check = Newtonsoft.Json.Linq.JObject.DeepEquals(descriptor, (Newtonsoft.Json.Linq.JObject)jObject);

            return check;
        }
    }
}
