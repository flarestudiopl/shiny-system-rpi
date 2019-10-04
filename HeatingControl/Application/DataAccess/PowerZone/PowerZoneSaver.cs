using Domain;
using Storage.StorageDatabase;
using System.Collections.Generic;
using System.Linq;

namespace HeatingControl.Application.DataAccess.PowerZone
{
    public interface IPowerZoneSaver
    {
        Domain.PowerZone Save(PowerZoneSaverInput input, Domain.Building building);
    }

    public class PowerZoneSaverInput
    {
        public int? PowerZoneId { get; set; }
        public string Name { get; set; }
        public ICollection<int> AffectedHeatersIds { get; set; }
        public decimal PowerLimitValue { get; set; }
        public UsageUnit PowerLimitUnit { get; set; }
        public int RoundRobinIntervalMinutes { get; set; }
    }

    public class PowerZoneSaver : IPowerZoneSaver
    {
        private readonly IDbExecutor _dbExecutor;

        public PowerZoneSaver(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public Domain.PowerZone Save(PowerZoneSaverInput input, Domain.Building building)
        {
            return _dbExecutor.Query(c =>
            {
                c.Attach(building);

                Domain.PowerZone powerZone = null;

                if (input.PowerZoneId.HasValue)
                {
                    powerZone = building.PowerZones.SingleOrDefault(z => z.PowerZoneId == input.PowerZoneId.Value);
                }

                if (powerZone == null)
                {
                    powerZone = new Domain.PowerZone { BuildingId = building.BuildingId };
                    building.PowerZones.Add(powerZone);
                }

                powerZone.Name = input.Name;
                powerZone.MaxUsage = input.PowerLimitValue;
                powerZone.UsageUnit = input.PowerLimitUnit;
                powerZone.RoundRobinIntervalMinutes = input.RoundRobinIntervalMinutes;

                MergeHeaters(input, c, powerZone);

                c.SaveChanges();

                return powerZone;
            });
        }

        private void MergeHeaters(PowerZoneSaverInput input, EfContext c, Domain.PowerZone powerZone)
        {
            var heaterIdsToAdd = input.AffectedHeatersIds.ToHashSet();
            var heatersToRemove = new List<Domain.Heater>();

            foreach (var zoneHeater in powerZone.Heaters)
            {
                if (heaterIdsToAdd.Contains(zoneHeater.HeaterId))
                {
                    heaterIdsToAdd.Remove(zoneHeater.HeaterId);
                }
                else
                {
                    heatersToRemove.Add(zoneHeater);
                }
            }

            foreach (var heaterToRemove in heatersToRemove)
            {
                heaterToRemove.PowerZone = null;
                heaterToRemove.PowerZoneId = null;

                powerZone.Heaters.Remove(heaterToRemove);
            }

            foreach (var heaterIdToAdd in heaterIdsToAdd)
            {
                var heaterToAdd = c.Heaters.Find(heaterIdToAdd);

                heaterToAdd.PowerZone = powerZone;
                powerZone.Heaters.Add(heaterToAdd);
            }
        }
    }
}
