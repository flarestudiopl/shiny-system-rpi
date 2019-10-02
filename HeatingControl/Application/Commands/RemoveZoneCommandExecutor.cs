using Commons.Extensions;
using Commons.Localization;
using Domain;
using HeatingControl.Application.DataAccess;

namespace HeatingControl.Application.Commands
{
    public class RemoveZoneCommand
    {
        public int ZoneId { get; set; }
    }

    public class RemoveZoneCommandExecutor : ICommandExecutor<RemoveZoneCommand>
    {
        private readonly IRepository<Zone> _zoneRepository;

        public RemoveZoneCommandExecutor(IRepository<Zone> zoneRepository)
        {
            _zoneRepository = zoneRepository;
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

            context.ControllerState.Model.Zones.Remove(x => x.ZoneId == command.ZoneId);
            _zoneRepository.Delete(zoneState.Zone);

            return CommandResult.Empty;
        }
    }
}
