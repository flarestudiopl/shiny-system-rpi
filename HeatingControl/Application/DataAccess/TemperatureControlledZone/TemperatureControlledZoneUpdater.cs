using HeatingControl.Models;
using Storage.StorageDatabase;
using System;

namespace HeatingControl.Application.DataAccess.TemperatureControlledZone
{
    public interface ITemperatureControlledZoneUpdater
    {
        void Update(TemperatureControlledZoneUpdaterInput temperatureControlledZoneUpdaterInput);
    }

    public class TemperatureControlledZoneUpdaterInput
    {
        public Domain.TemperatureControlledZone TemperatureControlledZone { get; set; }
        public SetPointType SetPointType { get; set; }
        public float Value { get; set; }
    }

    public class TemperatureControlledZoneUpdater : ITemperatureControlledZoneUpdater
    {
        private readonly IDbExecutor _dbExecutor;

        public TemperatureControlledZoneUpdater(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public void Update(TemperatureControlledZoneUpdaterInput temperatureControlledZoneUpdaterInput)
        {
            _dbExecutor.Execute(c =>
            {
                c.Attach(temperatureControlledZoneUpdaterInput.TemperatureControlledZone);

                switch (temperatureControlledZoneUpdaterInput.SetPointType)
                {
                    case SetPointType.Low:
                        temperatureControlledZoneUpdaterInput.TemperatureControlledZone.LowSetPoint = temperatureControlledZoneUpdaterInput.Value;
                        break;
                    case SetPointType.High:
                        temperatureControlledZoneUpdaterInput.TemperatureControlledZone.HighSetPoint = temperatureControlledZoneUpdaterInput.Value;
                        break;
                    case SetPointType.Schedule:
                        temperatureControlledZoneUpdaterInput.TemperatureControlledZone.ScheduleDefaultSetPoint = temperatureControlledZoneUpdaterInput.Value;
                        break;
                    case SetPointType.Hysteresis:
                        temperatureControlledZoneUpdaterInput.TemperatureControlledZone.Hysteresis = temperatureControlledZoneUpdaterInput.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                c.SaveChanges();
            });
        }
    }
}
