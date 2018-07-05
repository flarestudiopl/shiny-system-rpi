using Commons.Extensions;
using Commons.Localization;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public class RemoveZoneCommand
    {
        public int ZoneId { get; set; }
    }

    public class RemoveZoneCommandExecutor : ICommandExecutor<RemoveZoneCommand>
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public RemoveZoneCommandExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public CommandResult Execute(RemoveZoneCommand command, CommandContext context)
        {
            if (!context.ControllerState.ZoneIdToState.ContainsKey(command.ZoneId))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownZoneId.FormatWith(command.ZoneId));
            }

            var zoneState = context.ControllerState.ZoneIdToState[command.ZoneId];

            context.ControllerState.ZoneIdToState.Remove(command.ZoneId);

            foreach (var heaterToDisable in zoneState.Zone.HeaterIds)
            {
                context.ControllerState.HeaterIdToState[heaterToDisable].OutputState = false;
            }

            context.ControllerState.Model.Zones.Remove(x => x.ZoneId == command.ZoneId);

            _buildingModelSaver.Save(context.ControllerState.Model);

            return CommandResult.Empty;
        }
    }
}
