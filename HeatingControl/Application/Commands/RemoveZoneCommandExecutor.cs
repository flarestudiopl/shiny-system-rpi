using Commons.Extensions;
using Commons.Localization;
using HeatingControl.Application.DataAccess.Zone;

namespace HeatingControl.Application.Commands
{
    public class RemoveZoneCommand
    {
        public int ZoneId { get; set; }
    }

    public class RemoveZoneCommandExecutor : ICommandExecutor<RemoveZoneCommand>
    {
        private readonly IZoneRemover _zoneRemover;

        public RemoveZoneCommandExecutor(IZoneRemover zoneRemover)
        {
            _zoneRemover = zoneRemover;
        }

        public CommandResult Execute(RemoveZoneCommand command, CommandContext context)
        {
            if (!context.ControllerState.ZoneIdToState.ContainsKey(command.ZoneId))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownZoneId.FormatWith(command.ZoneId));
            }

            var zoneState = context.ControllerState.ZoneIdToState[command.ZoneId];

            context.ControllerState.ZoneIdToState.Remove(command.ZoneId);

            foreach (var heaterToDisable in zoneState.Zone.Heaters)
            {
                context.ControllerState.HeaterIdToState[heaterToDisable.HeaterId].OutputState = false;
            }

            _zoneRemover.Remove(zoneState.Zone, context.ControllerState.Model);

            return CommandResult.Empty;
        }
    }
}
