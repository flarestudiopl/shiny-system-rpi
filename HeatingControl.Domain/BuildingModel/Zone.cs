using System.Collections.Generic;

namespace Domain.BuildingModel
{
    public class Zone
    {
        public int ZoneId { get; set; }

        public string Name { get; set; }

        public HashSet<int> HeaterIds { get; set; } = new HashSet<int>();

        public IList<ScheduleItem> Schedule { get; set; } = new List<ScheduleItem>();

        public TemperatureControlledZone TemperatureControlledZone { get; set; }
    }
}
