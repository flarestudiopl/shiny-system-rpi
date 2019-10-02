using Commons.Extensions;
using Commons.Localization;
using Domain;
using HeatingControl.Application.DataAccess;
using HeatingControl.Models;

namespace HeatingControl.Application.Commands
{
    public class RemoveHeaterCommand
    {
        public int HeaterId { get; set; }
    }

    public class RemoveHeaterCommandExecutor : ICommandExecutor<RemoveHeaterCommand>
    {
        private readonly IRepository<Heater> _heaterRepository;

        public RemoveHeaterCommandExecutor(IRepository<Heater> heaterRepository)
        {
            _heaterRepository = heaterRepository;
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
            context.ControllerState.Model.Heaters.Remove(x=>x.HeaterId == command.HeaterId);

            _heaterRepository.Delete(heaterState.Heater);

            return CommandResult.Empty;
        }
    }
}
