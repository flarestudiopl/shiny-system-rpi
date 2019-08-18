using System;
using System.Collections.Generic;
using System.Linq;
using Domain.BuildingModel;
using HeatingControl.Extensions;
using HeatingControl.Models;
using Commons.Extensions;
using HeatingControl.Application.Loops.Processing;
using Domain.StorageDatabase;
using HeatingControl.Application.DataAccess;

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
        public ScheduleSettings Schedule { get; set; }

        public class CountersData
        {
            public IDictionary<UsageUnit, decimal> UsageUnitToValue { get; set; }
            public DateTime? LastResetDate { get; set; }
            public IDictionary<UsageUnit, Dictionary<string, decimal>> UsageUnitToHeaterNameToValue { get; set; }
        }

        public class TemperatureSettings
        {
            public float LowSetPoint { get; set; }
            public float HightSetPoint { get; set; }
            public float Hysteresis { get; set; }
            public IDictionary<DateTime, double> PlotData { get; set; }
        }

        public class ScheduleSettings
        {
            public float? DefaultSetPoint { get; set; }
            public ICollection<ScheduleItem> Items { get; set; }
        }
    }

    public class ZoneDetailsProvider : IZoneDetailsProvider
    {
        private readonly IRepository<Counter> _counterRepository;
        private readonly IZoneTemperatureProvider _zoneTemperatureProvider;

        public ZoneDetailsProvider(IRepository<Counter> counterRepository,
                                   IZoneTemperatureProvider zoneTemperatureProvider)
        {
            _counterRepository = counterRepository;
            _zoneTemperatureProvider = zoneTemperatureProvider;
        }

        public ZoneDetailsProviderResult Provide(int zoneId, ControllerState controllerState, Building building)
        {
            var zone = controllerState.ZoneIdToState.GetValueOrDefault(zoneId);

            if (zone == null)
            {
                return null;
            }

            return new ZoneDetailsProviderResult
            {
                Counters = GetCountersData(zone, controllerState, building),
                Temperatures = GetTemperatureSettings(zone, controllerState),
                Schedule = GetScheduleSettings(zone)
            };
        }

        private ZoneDetailsProviderResult.CountersData GetCountersData(ZoneState zone, ControllerState state, Building building)
        {
            var heaterIds = zone.Zone.HeaterIds;
            var heatersCounters = _counterRepository.Read(x => heaterIds.Contains(x.HeaterId) &&
                                                               !x.ResetDate.HasValue)
                                                    .ToDictionary(x => x.HeaterId,
                                                                  x => x);

            var now = DateTime.UtcNow;

            var heaterIdToCountedSeconds = heaterIds.ToDictionary(x => x,
                                                                  x =>
                                                                  {
                                                                      var savedCounterValue = DictionaryExtensions.GetValueOrDefault(heatersCounters, x)?.CountedSeconds ?? 0;
                                                                      var heaterState = state.HeaterIdToState[x];
                                                                      var currentCounterValue = heaterState.OutputState ? (int)(now - heaterState.LastCounterStart).TotalSeconds : 0;

                                                                      return savedCounterValue + currentCounterValue;
                                                                  });

            var usageUnitToHeaterToValue = building.Heaters
                                                   .Where(x => heaterIds.Contains(x.HeaterId))
                                                   .GroupBy(x => x.UsageUnit)
                                                   .ToDictionary(x => x.Key,
                                                                 x => x.ToDictionary(h => h.Name, h => h.UsagePerHour * (decimal)(heaterIdToCountedSeconds[h.HeaterId] / 3600f)));

            return new ZoneDetailsProviderResult.CountersData
            {
                LastResetDate = heatersCounters.Any() ? heatersCounters.Values.Min(x => x.StartDate) : (DateTime?)null,
                UsageUnitToValue = usageUnitToHeaterToValue.ToDictionary(x => x.Key,
                                                                                x => x.Value.Sum(h => h.Value)),
                UsageUnitToHeaterNameToValue = usageUnitToHeaterToValue
            };
        }

        private ZoneDetailsProviderResult.TemperatureSettings GetTemperatureSettings(ZoneState zone, ControllerState controllerState)
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
                Hysteresis = temperatureControlledZone.Hysteresis,
                PlotData = _zoneTemperatureProvider.Provide(zone.Zone.ZoneId, controllerState)
                                                          .HistoricalReads
                                                          .ToDictionary(x => x.Item1,
                                                                        x => x.Item2)
            };
        }

        private static ZoneDetailsProviderResult.ScheduleSettings GetScheduleSettings(ZoneState zone)
        {
            var scheduleSettings = new ZoneDetailsProviderResult.ScheduleSettings
            {
                Items = zone.Zone
                                                   .Schedule
                                                   .OrderBy(x => x.DaysOfWeek.FirstOrDefault())
                                                   .ThenBy(x => x.BeginTime)
                                                   .ToList()
            };

            if (zone.Zone.IsTemperatureControlled())
            {
                scheduleSettings.DefaultSetPoint = zone.Zone.TemperatureControlledZone.ScheduleDefaultSetPoint;
            }

            return scheduleSettings;
        }
    }
}
