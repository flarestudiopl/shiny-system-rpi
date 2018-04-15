using System.Collections.Generic;

namespace HeatingControl.Domain
{
    public class Zone
    {
        public string Name { get; set; }

        public IList<string> HeatersNames { get; set; } = new List<string>();

        public IList<ScheduleItem> Schedule { get; set; } = new List<ScheduleItem>();

        public TemperatureControlledZone TemperatureControlledZone { get; set; }
    }
}
