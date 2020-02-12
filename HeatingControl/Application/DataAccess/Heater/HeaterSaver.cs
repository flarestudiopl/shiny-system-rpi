using Commons;
using Commons.Extensions;
using Commons.Localization;
using Domain;
using Storage.StorageDatabase;

namespace HeatingControl.Application.DataAccess.Heater
{
    public interface IHeaterSaver
    {
        Domain.Heater Save(HeaterSaverInput input, Domain.Building building);
    }

    public class HeaterSaverInput
    {
        public int? HeaterId { get; set; }
        public string Name { get; set; }
        public object PowerOutputDescriptor { get; set; }
        public string PowerOutputProtocolName { get; set; }
        public UsageUnit UsageUnit { get; set; }
        public decimal UsagePerHour { get; set; }
        public int MinimumStateChangeIntervalSeconds { get; set; }
    }

    public class HeaterSaver : IHeaterSaver
    {
        private readonly IDbExecutor _dbExecutor;

        public HeaterSaver(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public Domain.Heater Save(HeaterSaverInput input, Domain.Building building)
        {
            return _dbExecutor.Query(c =>
            {
                c.Attach(building);

                Domain.Heater heater = null;

                if (input.HeaterId.HasValue)
                {
                    heater = c.Heaters.Find(input.HeaterId.Value);
                }

                if (heater == null)
                {
                    heater = new Domain.Heater { BuildingId = building.BuildingId, DigitalOutput = new DigitalOutput() };

                    building.Heaters.Add(heater);
                }
                else
                {
                    if (heater.PowerZoneId.HasValue &&
                        heater.UsageUnit != input.UsageUnit)
                    {
                        heater.PowerZone = null;
                        heater.PowerZoneId = null;

                        Logger.Info(Localization.NotificationMessage.HeaterRemovedFromPowerZoneDueToUsageUnitChange.FormatWith(input.Name));
                    }
                }

                heater.Name = input.Name;
                heater.UsageUnit = input.UsageUnit;
                heater.UsagePerHour = input.UsagePerHour;
                heater.MinimumStateChangeIntervalSeconds = input.MinimumStateChangeIntervalSeconds;
                heater.DigitalOutput.OutputDescriptor = Newtonsoft.Json.JsonConvert.SerializeObject(input.PowerOutputDescriptor);
                heater.DigitalOutput.ProtocolName = input.PowerOutputProtocolName;

                c.SaveChanges();

                return heater;
            });
        }
    }
}
