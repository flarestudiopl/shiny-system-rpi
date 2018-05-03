using System;
using System.Linq;
using HeatingControl.Domain;
using HeatingControl.Extensions;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public interface ITemperatureSetPointExecutor
    {
        void Execute(TemperatureSetPoinExecutorInput input, Building building);
    }

    public class TemperatureSetPoinExecutorInput
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

    public class TemperatureSetPointExecutor : ITemperatureSetPointExecutor
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public TemperatureSetPointExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public void Execute(TemperatureSetPoinExecutorInput input, Building building)
        {
            var zone = building.Zones.FirstOrDefault(x => x.ZoneId == input.ZoneId);

            if (zone == null || !zone.IsTemperatureControlled())
            {
                return;
            }

            var temperatureControlledZone = zone.TemperatureControlledZone;

            switch (input.SetPointType)
            {
                case SetPointType.Low:
                    temperatureControlledZone.LowSetPoint = input.Value;
                    break;
                case SetPointType.High:
                    temperatureControlledZone.HighSetPoint = input.Value;
                    break;
                case SetPointType.Schedule:
                    temperatureControlledZone.ScheduleDefaultSetPoint = input.Value;
                    break;
                case SetPointType.Hysteresis:
                    temperatureControlledZone.Hysteresis = input.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _buildingModelSaver.Save(building);
        }
    }
}
