using System;
using System.Collections.Generic;

namespace Domain
{
    public class Zone
    {
        public int ZoneId { get; set; }

        public int BuildingId { get; set; }

        public string Name { get; set; }

        [Obsolete("Temporary aproach")]
        public string NameDashboardEn { get; set; } // TODO - remove; how about a separate table with name for each lang?

        public int? TemperatureControlledZoneId { get; set; }

        public TemperatureControlledZone TemperatureControlledZone { get; set; }

        public ICollection<Heater> Heaters { get; set; } = new List<Heater>();

        public ICollection<ScheduleItem> Schedule { get; set; } = new List<ScheduleItem>();
    }
}
