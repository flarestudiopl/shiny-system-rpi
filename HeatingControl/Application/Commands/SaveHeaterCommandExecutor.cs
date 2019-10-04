using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Domain;
using HeatingControl.Application.DataAccess;
using HeatingControl.Models;

namespace HeatingControl.Application.Commands
{
    public class SaveHeaterCommand
    {
        public string Name { get; set; }
        public int PowerOutputDeviceId { get; set; }
        public string PowerOutputChannel { get; set; }
        public string PowerOutputProtocolName { get; set; }
        public UsageUnit UsageUnit { get; set; }
        public decimal UsagePerHour { get; set; }
        public int MinimumStateChangeIntervalSeconds { get; set; }
    }

    public class SaveHeaterCommandExecutor : ICommandExecutor<SaveHeaterCommand>
    {
        private readonly IRepository<Heater> _heaterRepository;

        public SaveHeaterCommandExecutor(IRepository<Heater> heaterRepository)
        {
            _heaterRepository = heaterRepository;
        }

        public CommandResult Execute(SaveHeaterCommand command, CommandContext context)
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

            if (context.ControllerState.Model.Heaters.Any(x => x.DigitalOutput.DeviceId == command.PowerOutputDeviceId &&
                                                               x.DigitalOutput.OutputChannel == command.PowerOutputChannel &&
                                                               x.DigitalOutput.ProtocolName == command.PowerOutputProtocolName))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.PowerOutputParametersAlreadyAssigned.FormatWith(command.PowerOutputDeviceId, command.PowerOutputChannel));
            }

            var heater = new Heater
            {
                Name = command.Name,
                BuildingId = context.ControllerState.Model.BuildingId,
                UsageUnit = command.UsageUnit,
                UsagePerHour = command.UsagePerHour,
                MinimumStateChangeIntervalSeconds = command.MinimumStateChangeIntervalSeconds,
                DigitalOutput = new DigitalOutput
                {
                    DeviceId = command.PowerOutputDeviceId,
                    OutputChannel = command.PowerOutputChannel,
                    ProtocolName = command.PowerOutputProtocolName,
                }
            };

            heater = _heaterRepository.Create(heater, context.ControllerState.Model);

            context.ControllerState.HeaterIdToState.Add(heater.HeaterId, new HeaterState
            {
                Heater = heater
            });

            return CommandResult.Empty;
        }
    }
}
