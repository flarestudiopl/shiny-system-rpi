using System;
using System.Collections.Generic;
using System.Linq;
using Domain.BuildingModel;
using HeatingControl.Extensions;
using HeatingControl.Models;
using Storage.StorageDatabase.Counter;

namespace HeatingControl.Application.Queries
{
    public interface IZoneDetailsProvider
    {
        ZoneDetailsProviderResult Provide(int zoneId, ControllerState controllerState, Building building);
    }

    public class ZoneDetailsProviderResult
    {
        public CountersData Counters { get; set; }
        public TemperatureSettings Temperatures { get; set; }
        public ICollection<ScheduleItem> Schedule { get; set; }

        public class CountersData
        {
            public IDictionary<UsageUnit, float> UsageUnitToValue { get; set; }
            public DateTime? LastResetDate { get; set; }
            public IDictionary<DateTime, float> PlotPoints { get; set; }
        }

        public class TemperatureSettings
        {
            public float LowSetPoint { get; set; }
            public float HightSetPoint { get; set; }
            public float ScheduleDefaultSetPoint { get; set; }
            public float Hysteresis { get; set; }
        }
    }

    public class ZoneDetailsProvider : IZoneDetailsProvider
    {
        private readonly ICurrentCountersByHeaterIdsProvider _currentCountersByHeaterIdsProvider;

        public ZoneDetailsProvider(ICurrentCountersByHeaterIdsProvider currentCountersByHeaterIdsProvider)
        {
            _currentCountersByHeaterIdsProvider = currentCountersByHeaterIdsProvider;
        }

        public ZoneDetailsProviderResult Provide(int zoneId, ControllerState controllerState, Building building)
        {
            var zone = controllerState.ZoneIdToState.GetValueOrDefault(zoneId);

            if (zone == null)
            {
                return new ZoneDetailsProviderResult();
            }

            return new ZoneDetailsProviderResult
                   {
                       Counters = GetCountersData(zone, building),
                       Temperatures = GetTemperatureSettings(zone),
                       Schedule = zone.Zone
                                      .Schedule
                                      .OrderBy(x => x.DayOfWeek)
                                      .ThenBy(x => x.BeginTime)
                                      .ToList()
                   };
        }

        private ZoneDetailsProviderResult.CountersData GetCountersData(ZoneState zone, Building building)
        {
            var heaterIds = zone.Zone.HeaterIds;
            var heatersCounters = _currentCountersByHeaterIdsProvider.Provide(heaterIds)
                                                                     .ToDictionary(x => x.HeaterId,
                                                                                   x => x);

            var heaterIdToCountedSeconds = heaterIds.ToDictionary(x => x,
                 x =>
                 {
                     var savedCounterValue = heatersCounters.GetValueOrDefault(x)?.CountedSeconds ?? 0;
                     // TODO - add current, not saved yet value
                     return savedCounterValue;
                 });

            var usageUnitToHeaters = building.Heaters
                                             .Where(x => heaterIds.Contains(x.HeaterId))
                                             .GroupBy(x => x.UsageUnit);

            return new ZoneDetailsProviderResult.CountersData
                    {
                        LastResetDate = heatersCounters.Any() ? heatersCounters.Values.Min(x => x.Start) : (DateTime?)null,
                        UsageUnitToValue = usageUnitToHeaters.ToDictionary(x => x.Key,
                                                                           x => x.Sum(h => h.UsagePerHour * (float)heaterIdToCountedSeconds[h.HeaterId] / 3600f))
                    };
        }

        private static ZoneDetailsProviderResult.TemperatureSettings GetTemperatureSettings(ZoneState zone)
        {
            if (!zone.Zone.IsTemperatureControlled())
            {
                return null;
            }

            var temperatureControlledZone = zone.Zone.TemperatureControlledZone;

            return new ZoneDetailsProviderResult.TemperatureSettings
                   {
                       HightSetPoint = temperatureControlledZone.HighSetPoint,
                       LowSetPoint = temperatureControlledZone.LowSetPoint,
                       ScheduleDefaultSetPoint = temperatureControlledZone.ScheduleDefaultSetPoint,
                       Hysteresis = temperatureControlledZone.Hysteresis
                   };
        }
    }
}
