using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Domain;
using HeatingControl.Application.DataAccess;

namespace HeatingControl.Application.Commands
{
    public class RemoveTemperatureSensorCommand
    {
        public int SensorId { get; set; }
    }

    public class RemoveTemperatureSensorCommandExecutor : ICommandExecutor<RemoveTemperatureSensorCommand>
    {
        private readonly IRepository<TemperatureSensor> _temperatureSensorRepository;

        public RemoveTemperatureSensorCommandExecutor(IRepository<TemperatureSensor> temperatureSensorRepository)
        {
            _temperatureSensorRepository = temperatureSensorRepository;
        }

        public CommandResult Execute(RemoveTemperatureSensorCommand command, CommandContext context)
        {
            var temperatureSensor = context.ControllerState.Model.TemperatureSensors.SingleOrDefault(x => x.TemperatureSensorId == command.SensorId);

            if (temperatureSensor == null)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownTemperatureSensorId.FormatWith(command.SensorId));
            }

            if (context.ControllerState.Model.Zones.Any(x => x.TemperatureControlledZone?.TemperatureSensorId == command.SensorId))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.CantDeleteSensorAssignedToZone);
            }

            context.ControllerState.TemperatureSensorIdToState.Remove(command.SensorId);

            _temperatureSensorRepository.Delete(temperatureSensor, context.ControllerState.Model);

            return CommandResult.Empty;
        }
    }
}
