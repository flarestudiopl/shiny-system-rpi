using HeatingControl.Domain;
using System;
using System.Collections.Generic;

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
        public ICollection<Zone> Zones { get; set; }
        public ICollection<Notification> Notifications { get; set; }

        public class Zone
        {
            public string Name { get; set; }
            public TemperatureControlledZone TemperatureControl {get;set;}
            public OnOffControlledZone OnOffControl { get; set; }
            public ZoneControlMode ControlMode { get; set; }
            public bool OutputState { get; set; }

            public class TemperatureControlledZone
            {
                public float Temperature { get; set; }
                public float LowSetPoint { get; set; }
                public float HighSetPoint { get; set; }
                public float ScheduleSetPoint { get; set; }
            }

            public class OnOffControlledZone
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

        public DashboardSnapshotProvider(IHeatingControl heatingControl)
        {
            _heatingControl = heatingControl;
        }

        public DashboardSnapshotProviderOutput Provide()
        {
            throw new NotImplementedException();
        }
    }
}
