using Commons.Extensions;
using Commons.Localization;
using HeatingControl.Application.DataAccess;

namespace HeatingControl.Application.Commands
{
    public class RemovePowerZoneCommand
    {
        public int PowerZoneId { get; set; }
    }

    public class RemovePowerZoneCommandExecutor : ICommandExecutor<RemovePowerZoneCommand>
    {
        private readonly IRepository<Domain.PowerZone> _powerZoneRepository;

        public RemovePowerZoneCommandExecutor(IRepository<Domain.PowerZone> powerZoneRepository)
        {
            _powerZoneRepository = powerZoneRepository;
        }

        public CommandResult Execute(RemovePowerZoneCommand command, CommandContext context)
        {
            var powerZoneState = context.ControllerState.PowerZoneIdToState.GetValueOrDefault(command.PowerZoneId);

            if (powerZoneState == null)
            {
               return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownPowerZoneId.FormatWith(command.PowerZoneId));
            }

            context.ControllerState.PowerZoneIdToState.Remove(command.PowerZoneId);
            context.ControllerState.Model.PowerZones.Remove(x => x.PowerZoneId == command.PowerZoneId);

            _powerZoneRepository.Delete(powerZoneState.PowerZone);
            
            return CommandResult.Empty;
        }
    }
}
