using HeatingControl.Domain;
using HeatingControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeatingControl.Application
{
    public interface IDashboardSnapshotProvider
    {
        DashboardSnapshotProviderOutput Provide();
    }

    public class DashboardSnapshotProviderOutput
    {
        public DateTime ControllerTime { get; set; }
        public string BuildingName { get; set; }
        public string InstantConsumptionFormatted { get; set; }
        public ICollection<ZoneSnapshot> Zones { get; set; }
        public ICollection<Notification> Notifications { get; set; }

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
                public float Temperature { get; set; }
                public float LowSetPoint { get; set; }
                public float HighSetPoint { get; set; }
                public float ScheduleSetPoint { get; set; }
            }

            public class OnOffControlSnapshot
            {
                public bool ScheduleState { get; set; }
            }
        }

        public class Notification
        {
            public string Message { get; set; }
        }
    }

    public class DashboardSnapshotProvider : IDashboardSnapshotProvider
    {
        private readonly IHeatingControl _heatingControl;
        private readonly IActualScheduleItemProvider _actualScheduleItemProvider;

        public DashboardSnapshotProvider(IHeatingControl heatingControl,
                                         IActualScheduleItemProvider actualScheduleItemProvider)
        {
            _heatingControl = heatingControl;
            _actualScheduleItemProvider = actualScheduleItemProvider;
        }

        public DashboardSnapshotProviderOutput Provide()
        {
            var model = _heatingControl.Model;
            var state = _heatingControl.State;

            var output = new DashboardSnapshotProviderOutput
            {
                BuildingName = model.Name,
                ControllerTime = DateTime.Now,
                Notifications = null, // TODO
                InstantConsumptionFormatted = null, // TODO
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

            if (zoneState.Zone.TemperatureControlledZone != null)
            {
                zoneSnapshot.TemperatureControl = new DashboardSnapshotProviderOutput.ZoneSnapshot.TemperatureControlSnapshot
                {
                    HighSetPoint = zoneState.Zone.TemperatureControlledZone.HighSetPoint,
                    LowSetPoint = zoneState.Zone.TemperatureControlledZone.LowSetPoint,
                    //Temperature = state.DeviceIdToTemperatureData[state.TemperatureSensorIdToDeviceId[zoneState.Zone.TemperatureControlledZone.TemperatureSensorId]].AverageTemperature,
                    ScheduleSetPoint = _actualScheduleItemProvider.TryProvide(zoneState.Zone.Schedule)?.SetPoint ?? zoneState.Zone.TemperatureControlledZone.ScheduleDefaultSetPoint // TODO - duplication in OutputStateProcessingLoop
                };
            }
            else
            {
                zoneSnapshot.OnOffControl = new DashboardSnapshotProviderOutput.ZoneSnapshot.OnOffControlSnapshot
                {
                    ScheduleState = _actualScheduleItemProvider.TryProvide(zoneState.Zone.Schedule) != null
                };
            }

            return zoneSnapshot;
        }
    }
}
