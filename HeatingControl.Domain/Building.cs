using System.Collections.Generic;

namespace HeatingControl.Domain
{
    public class Building
    {
        public string Name { get; set; }

        public IList<TemperatureZone> TemperatureZones { get; set; }

        public IList<PowerZone> PowerZones { get; set; }

        public int ControlLoopIntervalSecondsMilliseconds { get; set; }
    }
}
