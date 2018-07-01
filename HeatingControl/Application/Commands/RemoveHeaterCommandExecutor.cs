using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public class RemoveHeaterCommand
    {
        public int HeaterId { get; set; }
    }

    public class RemoveHeaterCommandExecutor : ICommandExecutor<RemoveHeaterCommand>
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public RemoveHeaterCommandExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public CommandResult Execute(RemoveHeaterCommand command, CommandContext context)
        {
            if (!context.ControllerState.HeaterIdToState.ContainsKey(command.HeaterId))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownHeaterId.FormatWith(command.HeaterId));
            }

            if (context.ControllerState.ZoneIdToState.Values.Any(x => x.Zone.HeaterIds.Contains(command.HeaterId)))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.CantDeleteHeaterAssignedToZone);
            }

            if (context.ControllerState.PowerZoneIdToState.Values.Any(x => x.PowerZone.HeaterIds.Contains(command.HeaterId)))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.CantDeleteHeaterAssignedToPowerZone);
            }

            context.ControllerState.HeaterIdToState.Remove(command.HeaterId);
            context.ControllerState.Model.Heaters.Remove(x=>x.HeaterId == command.HeaterId);

            _buildingModelSaver.Save(context.ControllerState.Model);

            return CommandResult.Empty;
        }
    }
}
