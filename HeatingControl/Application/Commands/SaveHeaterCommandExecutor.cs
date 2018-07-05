using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public class SaveHeaterCommand
    {
        public string Name { get; set; }
        public int PowerOutputDeviceId { get; set; }
        public int PowerOutputChannel { get; set; }
        public UsageUnit UsageUnit { get; set; }
        public decimal UsagePerHour { get; set; }
        public int MinimumStateChangeIntervalSeconds { get; set; }
    }

    public class SaveHeaterCommandExecutor : ICommandExecutor<SaveHeaterCommand>
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public SaveHeaterCommandExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public CommandResult Execute(SaveHeaterCommand command, CommandContext context)
        {
            if (command.Name.IsNullOrEmpty())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.NameCantBeEmpty);
            }

            if (context.ControllerState.Model.Heaters.Any(x => x.PowerOutputDeviceId == command.PowerOutputDeviceId &&
                                                               x.PowerOutputChannel == command.PowerOutputChannel))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.PowerOutputParametersAlreadyAssigned.FormatWith(command.PowerOutputDeviceId, command.PowerOutputChannel));
            }

            var heater = new Heater
                         {
                             HeaterId = (context.ControllerState.HeaterIdToState.Keys.Any() ? context.ControllerState.HeaterIdToState.Keys.Max() : 0) + 1,
                             Name = command.Name,
                             PowerOutputDeviceId = command.PowerOutputDeviceId,
                             PowerOutputChannel = command.PowerOutputChannel,
                             UsageUnit = command.UsageUnit,
                             UsagePerHour = command.UsagePerHour,
                             MinimumStateChangeIntervalSeconds = command.MinimumStateChangeIntervalSeconds
                         };

            context.ControllerState.HeaterIdToState.Add(heater.HeaterId, new HeaterState
                                                                         {
                                                                             Heater = heater
                                                                         });

            context.ControllerState.Model.Heaters.Add(heater);
            _buildingModelSaver.Save(context.ControllerState.Model);

            return CommandResult.Empty;
        }
    }
}
