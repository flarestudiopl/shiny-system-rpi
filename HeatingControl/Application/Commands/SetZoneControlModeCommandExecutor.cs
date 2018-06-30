using Commons.Extensions;
using Commons.Localization;
using Domain.BuildingModel;
using HeatingControl.Models;

namespace HeatingControl.Application.Commands
{
    public class SetZoneControlModeCommand
    {
        public int ZoneId { get; set; }
        public ZoneControlMode ControlMode { get; set; }
    }

    public class SetZoneControlModeCommandExecutor : ICommandExecutor<SetZoneControlModeCommand>
    {
        public CommandResult Execute(SetZoneControlModeCommand command, ControllerState state)
        {
            var zone = state.ZoneIdToState.GetValueOrDefault(command.ZoneId);

            if (zone == null)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownZoneId.FormatWith(command.ZoneId));
            }

            zone.ControlMode = command.ControlMode;

            return CommandResult.Empty;
        }
    }
}
