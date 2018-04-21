using HeatingControl.Domain;
using HeatingControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Commons;
using HeatingControl.Extensions;

namespace HeatingControl.Application
{
    public interface IDashboardSnapshotProvider
    {
        DashboardSnapshotProviderOutput Provide(Building model, ControllerState state);
    }

    public class DashboardSnapshotProviderOutput
    {
        public DateTime ControllerTime { get; set; }
        public string BuildingName { get; set; }
        public IDictionary<UsageUnit, float> InstantUsage { get; set; }
        public ICollection<ZoneSnapshot> Zones { get; set; }
        public ICollection<Logger.Message> Notifications { get; set; }

        public class ZoneSnapshot
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public TemperatureControlSnapshot TemperatureControl { get; set; }
            public OnOffControlSnapshot OnOffControl { get; set; }
            public ZoneControlMode ControlMode { get; set; }
            public bool OutputState { get; set; }

            public class TemperatureControlSnapshot
            {
                public float? Temperature { get; set; }
                public float LowSetPoint { get; set; }
                public float HighSetPoint { get; set; }
                public float ScheduleSetPoint { get; set; }
            }

            public class OnOffControlSnapshot
            {
                public bool ScheduleState { get; set; }
            }
        }
    }

    public class DashboardSnapshotProvider : IDashboardSnapshotProvider
    {
        private readonly IZoneTemperatureProvider _zoneTemperatureProvider;

        public DashboardSnapshotProvider(IZoneTemperatureProvider zoneTemperatureProvider)
        {
            _zoneTemperatureProvider = zoneTemperatureProvider;
        }

        public DashboardSnapshotProviderOutput Provide(Building model, ControllerState state)
        {
            var output = new DashboardSnapshotProviderOutput
                         {
                             BuildingName = model.Name,
                             ControllerTime = DateTime.Now,
                             Notifications = Logger.LastMessages.ToArray(),
                             InstantUsage = state.InstantUsage,
                             Zones = state.ZoneIdToState.Values.Select(x => BuildZoneSnapshot(x, state)).ToList()
                         };

            return output;
        }

        private DashboardSnapshotProviderOutput.ZoneSnapshot BuildZoneSnapshot(ZoneState zoneState, ControllerState state)
        {
            var zoneSnapshot = new DashboardSnapshotProviderOutput.ZoneSnapshot
                               {
                                   Id = zoneState.Zone.ZoneId,
                                   Name = zoneState.Zone.Name,
                                   ControlMode = zoneState.ControlMode,
                                   OutputState = zoneState.EnableOutputs
                               };

            if (zoneState.Zone.IsTemperatureControlled())
            {
                zoneSnapshot.TemperatureControl = new DashboardSnapshotProviderOutput.ZoneSnapshot.TemperatureControlSnapshot
                                                  {
                                                      HighSetPoint = zoneState.Zone.TemperatureControlledZone.HighSetPoint,
                                                      LowSetPoint = zoneState.Zone.TemperatureControlledZone.LowSetPoint,
                                                      Temperature = _zoneTemperatureProvider.Provide(zoneState.Zone.ZoneId, state)?.AverageTemperature,
                                                      ScheduleSetPoint = zoneState.ScheduleState.DesiredTemperature.Value
                                                  };
            }
            else
            {
                zoneSnapshot.OnOffControl = new DashboardSnapshotProviderOutput.ZoneSnapshot.OnOffControlSnapshot
                                            {
                                                ScheduleState = zoneState.ScheduleState.HeatingEnabled.Value
                                            };
            }

            return zoneSnapshot;
        }
    }
}
