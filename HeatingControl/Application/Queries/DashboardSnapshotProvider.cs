using System.Collections.Generic;
using System.Linq;
using HeatingControl.Application.Loops.Processing;
using Domain;
using HeatingControl.Extensions;
using HeatingControl.Models;
using Commons.Extensions;
using System;

namespace HeatingControl.Application.Queries
{
    public interface IDashboardSnapshotProvider
    {
        DashboardSnapshotProviderOutput Provide(Building model, ControllerState state);
    }

    public class DashboardSnapshotProviderOutput
    {
        public string BuildingName { get; set; }
        public DateTime ControllerDateTime { get; set; }
        public bool ControlEnabled { get; set; }
        public bool BatteryMode { get; set; }
        public DateTime? ScheduledShutdownUtcTime { get; set; }
        public string SoftwareVersion { get; set; }
        public IDictionary<UsageUnit, decimal> InstantUsage { get; set; }
        public ICollection<ZoneSnapshot> Zones { get; set; }

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
                public double? Temperature { get; set; }
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
        private readonly string AssemblyVersion;

        private readonly IZoneTemperatureProvider _zoneTemperatureProvider;

        public DashboardSnapshotProvider(IZoneTemperatureProvider zoneTemperatureProvider)
        {
            _zoneTemperatureProvider = zoneTemperatureProvider;

            AssemblyVersion = typeof(DashboardSnapshotProvider).Assembly.GetName().Version.ToString();
        }

        public DashboardSnapshotProviderOutput Provide(Building model, ControllerState state)
        {
            var output = new DashboardSnapshotProviderOutput
                         {
                             BuildingName = model.Name,
                             ControllerDateTime = DateTime.UtcNow,
                             SoftwareVersion = AssemblyVersion,
                             ControlEnabled = state.ControlEnabled,
                             BatteryMode = state.DigitalInputFunctionToState.GetValueOrDefault(DigitalInputFunction.BatteryMode)?.State ?? false,
                             ScheduledShutdownUtcTime = state.ScheduledShutdownUtcTime,
                             InstantUsage = state.InstantUsage,
                             Zones = state.ZoneIdToState
                                          .Values
                                          .Select(x => BuildZoneSnapshot(x, state))
                                          .OrderBy(x => x.Name)
                                          .ToList()
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
                                                ScheduleState = zoneState.ScheduleState.HeatingEnabled ?? false
                                            };
            }

            return zoneSnapshot;
        }
    }
}
