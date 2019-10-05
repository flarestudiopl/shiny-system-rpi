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

            if (command.DeviceId.IsNullOrEmpty())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.DeviceIdCantBeEmpty);
            }

            var existingTemperatureSensor = context.ControllerState.Model.TemperatureSensors?.FirstOrDefault(x => x.DeviceId == command.DeviceId);

            if (existingTemperatureSensor != null)
            {
                if (!command.Id.HasValue || command.Id.Value != existingTemperatureSensor.TemperatureSensorId)
                {
                    return CommandResult.WithValidationError(Localization.ValidationMessage.DeviceIdAlreadyInUse);
                }
            }

            var temperatureSensor = _temperatureSensorSaver.Save(command, context.ControllerState.Model);

            context.ControllerState.TemperatureSensorIdToDeviceId[temperatureSensor.TemperatureSensorId] = temperatureSensor.DeviceId;

            return CommandResult.Empty;
        }
    }
}
