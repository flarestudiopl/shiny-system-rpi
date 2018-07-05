using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public class RemoveTemperatureSensorCommand
    {
        public int SensorId { get; set; }
    }

    public class RemoveTemperatureSensorCommandExecutor : ICommandExecutor<RemoveTemperatureSensorCommand>
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public RemoveTemperatureSensorCommandExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public CommandResult Execute(RemoveTemperatureSensorCommand command, CommandContext context)
        {
            if (!context.ControllerState.TemperatureSensorIdToDeviceId.ContainsKey(command.SensorId))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownTemperatureSensorId.FormatWith(command.SensorId));
            }

            if (context.ControllerState.Model.Zones.Any(x => x.TemperatureControlledZone?.TemperatureSensorId == command.SensorId))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.CantDeleteSensorAssignedToZone);
            }

            context.ControllerState.TemperatureSensorIdToDeviceId.Remove(command.SensorId);
            context.ControllerState.Model.TemperatureSensors.Remove(x => x.TemperatureSensorId == command.SensorId);

            _buildingModelSaver.Save(context.ControllerState.Model);

            return CommandResult.Empty;
        }
    }
}
