using System.Collections.Generic;

namespace Domain
{
    public class Zone
    {
        public int ZoneId { get; set; }

        public int BuildingId { get; set; }

        public string Name { get; set; }

        public int? TemperatureControlledZoneId { get; set; }

        public TemperatureControlledZone TemperatureControlledZone { get; set; }

        public ICollection<Heater> Heaters { get; set; } = new List<Heater>();

        public ICollection<ScheduleItem> Schedule { get; set; } = new List<ScheduleItem>();
    }
}
