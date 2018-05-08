using System;
using System.Collections.Generic;
using HeatingControl.Domain;
using HeatingControl.Extensions;
using HeatingControl.Models;

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
            public float UsageValue { get; set; }
            public UsageUnit UsageUnit { get; set; }
            public DateTime LastResetDate { get; set; }
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
        public ZoneDetailsProviderResult Provide(int zoneId, ControllerState controllerState, Building building)
        {
            var zone = controllerState.ZoneIdToState.GetValueOrDefault(zoneId);

            if (zone == null)
            {
                return new ZoneDetailsProviderResult();
            }

            return new ZoneDetailsProviderResult
                   {
                       Counters = GetCountersData(zone),
                       Temperatures = GetTemperatureSettings(zone),
                       Schedule = zone.Zone.Schedule
                   };
        }

        private static ZoneDetailsProviderResult.CountersData GetCountersData(ZoneState zone)
        {
            return new ZoneDetailsProviderResult.CountersData(); // TODO
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
