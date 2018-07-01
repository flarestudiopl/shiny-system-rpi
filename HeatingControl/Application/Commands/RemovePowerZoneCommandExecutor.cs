using Commons.Extensions;
using Commons.Localization;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public class RemovePowerZoneCommand
    {
        public int PowerZoneId { get; set; }
    }

    public class RemovePowerZoneCommandExecutor : ICommandExecutor<RemovePowerZoneCommand>
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public RemovePowerZoneCommandExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public CommandResult Execute(RemovePowerZoneCommand command, CommandContext context)
        {
            if (!context.ControllerState.PowerZoneIdToState.ContainsKey(command.PowerZoneId))
            {
               return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownPowerZoneId.FormatWith(command.PowerZoneId));
            }

            context.ControllerState.PowerZoneIdToState.Remove(command.PowerZoneId);
            context.ControllerState.Model.PowerZones.Remove(x => x.PowerZoneId == command.PowerZoneId);

            _buildingModelSaver.Save(context.ControllerState.Model);

            return CommandResult.Empty;
        }
    }
}
