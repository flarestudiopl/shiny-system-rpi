using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Domain;
using HeatingControl.Application.DataAccess;

namespace HeatingControl.Application.Commands
{
    public class SaveTemperatureSensorCommand
    {
        public string Name { get; set; }
        public string DeviceId { get; set; }
    }

    public class SaveTemperatureSensorCommandExecutor : ICommandExecutor<SaveTemperatureSensorCommand>
    {
        private readonly IRepository<TemperatureSensor> _temperatureSensorRepository;

        public SaveTemperatureSensorCommandExecutor(IRepository<TemperatureSensor> temperatureSensorRepository)
        {
            _temperatureSensorRepository = temperatureSensorRepository;
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

            if (context.ControllerState.Model.TemperatureSensors?.Any(x => x.DeviceId == command.DeviceId) ?? false)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.DeviceIdAlreadyInUse);
            }

            var temperatureSensor = new TemperatureSensor
                                    {
                                        Name = command.Name,
                                        BuildingId = context.ControllerState.Model.BuildingId,
                                        DeviceId = command.DeviceId
                                    };

            temperatureSensor = _temperatureSensorRepository.Create(temperatureSensor, context.ControllerState.Model);

            context.ControllerState.TemperatureSensorIdToDeviceId.Add(temperatureSensor.TemperatureSensorId, command.DeviceId);

            return CommandResult.Empty;
        }
    }
}
