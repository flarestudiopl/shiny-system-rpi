using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public class SaveTemperatureSensorCommand
    {
        public string Name { get; set; }
        public string DeviceId { get; set; }
    }

    public class SaveTemperatureSensorCommandExecutor : ICommandExecutor<SaveTemperatureSensorCommand>
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public SaveTemperatureSensorCommandExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
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

            if (context.ControllerState.Model.TemperatureSensors.Any(x => x.DeviceId == command.DeviceId))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.DeviceIdAlreadyInUse);
            }

            if (!context.ControllerState.TemperatureDeviceIdToTemperatureData.ContainsKey(command.DeviceId))
            {
                context.ControllerState.TemperatureDeviceIdToTemperatureData.Add(command.DeviceId, new TemperatureData());
            }

            var temperatureSensor = new TemperatureSensor
                                    {
                                        TemperatureSensorId = (context.ControllerState.TemperatureSensorIdToDeviceId.Keys.Any()
                                                                   ? context.ControllerState.TemperatureSensorIdToDeviceId.Keys.Max()
                                                                   : 0) + 1,
                                        Name = command.Name,
                                        DeviceId = command.DeviceId
                                    };

            context.ControllerState.TemperatureSensorIdToDeviceId.Add(temperatureSensor.TemperatureSensorId, command.DeviceId);

            context.ControllerState.Model.TemperatureSensors.Add(temperatureSensor);

            _buildingModelSaver.Save(context.ControllerState.Model);

            return CommandResult.Empty;
        }
    }
}
