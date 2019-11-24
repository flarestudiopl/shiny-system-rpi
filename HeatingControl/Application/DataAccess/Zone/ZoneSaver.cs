using System.Collections.Generic;
using System.Linq;
using Commons;
using Commons.Extensions;
using Commons.Localization;
using Domain;
using HeatingControl.Extensions;
using Storage.StorageDatabase;

namespace HeatingControl.Application.DataAccess.Zone
{
    public interface IZoneSaver
    {
        Domain.Zone Save(ZoneSaverInput input, Domain.Building building);
    }

    public class ZoneSaverInput
    {
        public int? ZoneId { get; set; }
        public string Name { get; set; }
        public int SwitchDelay { get; set; }
        public int? TemperatureSensorId { get; set; }
        public ICollection<int> HeaterIds { get; set; }
    }

    public class ZoneSaver : IZoneSaver
    {
        private readonly IDbExecutor _dbExecutor;

        public ZoneSaver(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public Domain.Zone Save(ZoneSaverInput input, Domain.Building building)
        {
            return _dbExecutor.Query(c =>
            {
                c.Attach(building);

                Domain.Zone zone = null;

                if (input.ZoneId.HasValue)
                {
                    zone = building.Zones.SingleOrDefault(z => z.ZoneId == input.ZoneId.Value);

                    TryClearSchedule(input, zone);
                }

                if (zone == null)
                {
                    zone = new Domain.Zone { BuildingId = building.BuildingId };
                    building.Zones.Add(zone);
                }

                zone.Name = input.Name;
                zone.SwitchDelayBetweenOutputsSeconds = input.SwitchDelay;

                MergeTemperatureControlledZone(input, c, zone);
                MergeHeaters(input, c, zone);

                c.SaveChanges();

                return zone;
            });
        }

        private static void TryClearSchedule(ZoneSaverInput input, Domain.Zone zone)
        {
            if (zone != null && zone.Schedule.Count > 0 && zone.IsTemperatureControlled() != input.TemperatureSensorId.HasValue)
            {
                zone.Schedule.Clear();

                Logger.Info(Localization.NotificationMessage.ScheduledRemovedDueToControlTypeChange.FormatWith(input.Name));
            }
        }

        private static void MergeTemperatureControlledZone(ZoneSaverInput input, EfContext c, Domain.Zone zone)
        {
            if (input.TemperatureSensorId.HasValue)
            {
                zone.TemperatureControlledZone = zone.TemperatureControlledZone ?? new Domain.TemperatureControlledZone
                {
                    LowSetPoint = 4.0f,
                    HighSetPoint = 20.0f,
                    Hysteresis = 0.5f,
                    ScheduleDefaultSetPoint = 4.0f
                };

                zone.TemperatureControlledZone.TemperatureSensor = c.TemperatureSensors.Find(input.TemperatureSensorId.Value);
                zone.TemperatureControlledZone.TemperatureSensorId = zone.TemperatureControlledZone.TemperatureSensor.TemperatureSensorId;
            }
            else if(zone.TemperatureControlledZone != null)
            {
                c.Remove(zone.TemperatureControlledZone);

                zone.TemperatureControlledZoneId = null;
                zone.TemperatureControlledZone = null;
            }
        }

        private static void MergeHeaters(ZoneSaverInput input, EfContext c, Domain.Zone zone)
        {
            var heaterIdsToAdd = input.HeaterIds.ToHashSet();
            var heatersToRemove = new List<Domain.Heater>();

            foreach (var zoneHeater in zone.Heaters)
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
                heaterToRemove.Zone = null;
                heaterToRemove.ZoneId = null;

                zone.Heaters.Remove(heaterToRemove);
            }

            foreach (var heaterIdToAdd in heaterIdsToAdd)
            {
                var heaterToAdd = c.Heaters.Find(heaterIdToAdd);

                heaterToAdd.Zone = zone;
                zone.Heaters.Add(heaterToAdd);
            }
        }
    }
}
