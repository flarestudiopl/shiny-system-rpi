using System;
using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using HeatingControl.Extensions;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public class SetTemperatureCommand
    {
        public int ZoneId { get; set; }
        public SetPointType SetPointType { get; set; }
        public float Value { get; set; }
    }

    public enum SetPointType
    {
        Low,
        High,
        Schedule,
        Hysteresis
    }

    public class SetTemperatureCommandExecutor : ICommandExecutor<SetTemperatureCommand>
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public SetTemperatureCommandExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public CommandResult Execute(SetTemperatureCommand command, ControllerState controllerState)
        {
            var zone = controllerState.Model.Zones.FirstOrDefault(x => x.ZoneId == command.ZoneId);

            if (zone == null || !zone.IsTemperatureControlled())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownZoneId.FormatWith(command.ZoneId));
            }

            var temperatureControlledZone = zone.TemperatureControlledZone;

            switch (command.SetPointType)
            {
                case SetPointType.Low:
                    temperatureControlledZone.LowSetPoint = command.Value;
                    break;
                case SetPointType.High:
                    temperatureControlledZone.HighSetPoint = command.Value;
                    break;
                case SetPointType.Schedule:
                    temperatureControlledZone.ScheduleDefaultSetPoint = command.Value;
                    break;
                case SetPointType.Hysteresis:
                    temperatureControlledZone.Hysteresis = command.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _buildingModelSaver.Save(controllerState.Model);

            return CommandResult.Empty;
        }
    }
}
