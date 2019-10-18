using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using HeatingControl.Application.DataAccess.TemperatureControlledZone;
using HeatingControl.Extensions;
using HeatingControl.Models;

namespace HeatingControl.Application.Commands
{
    public class SetTemperatureCommand
    {
        public int ZoneId { get; set; }
        public SetPointType SetPointType { get; set; }
        public float Value { get; set; }
    }

    public class SetTemperatureCommandExecutor : ICommandExecutor<SetTemperatureCommand>
    {
        private readonly ITemperatureControlledZoneUpdater _temperatureControlledZoneUpdater;

        public SetTemperatureCommandExecutor(ITemperatureControlledZoneUpdater temperatureControlledZoneUpdater)
        {
            _temperatureControlledZoneUpdater = temperatureControlledZoneUpdater;
        }

        public CommandResult Execute(SetTemperatureCommand command, CommandContext context)
        {
            var zone = context.ControllerState.Model.Zones.FirstOrDefault(x => x.ZoneId == command.ZoneId);

            if (zone == null || !zone.IsTemperatureControlled())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownZoneId.FormatWith(command.ZoneId));
            }

            if (command.SetPointType == SetPointType.Hysteresis && command.Value < 0.2f)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.HysteresisTooLow);
            }

            var temperatureControlledZone = zone.TemperatureControlledZone;

            _temperatureControlledZoneUpdater.Update(new TemperatureControlledZoneUpdaterInput
            {
                TemperatureControlledZone = zone.TemperatureControlledZone,
                SetPointType = command.SetPointType,
                Value = command.Value
            });

            return CommandResult.Empty;
        }
    }
}
