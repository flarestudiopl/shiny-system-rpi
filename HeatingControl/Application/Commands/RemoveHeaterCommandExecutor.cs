using Commons.Extensions;
using Commons.Localization;
using HeatingControl.Application.DataAccess.Heater;
using HeatingControl.Models;

namespace HeatingControl.Application.Commands
{
    public class RemoveHeaterCommand
    {
        public int HeaterId { get; set; }
    }

    public class RemoveHeaterCommandExecutor : ICommandExecutor<RemoveHeaterCommand>
    {
        private readonly IHeaterRemover _heaterRemover;

        public RemoveHeaterCommandExecutor(IHeaterRemover heaterRemover)
        {
            _heaterRemover = heaterRemover;
        }

        public CommandResult Execute(RemoveHeaterCommand command, CommandContext context)
        {
            HeaterState heaterState;

            if (!context.ControllerState.HeaterIdToState.TryGetValue(command.HeaterId, out heaterState))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownHeaterId.FormatWith(command.HeaterId));
            }

            if (heaterState.Heater.ZoneId.HasValue)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.CantDeleteHeaterAssignedToZone);
            }

            if (heaterState.Heater.PowerZoneId.HasValue)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.CantDeleteHeaterAssignedToPowerZone);
            }

            context.ControllerState.HeaterIdToState.Remove(command.HeaterId);

            _heaterRemover.Remove(heaterState.Heater, context.ControllerState.Model);

            return CommandResult.Empty;
        }
    }
}
