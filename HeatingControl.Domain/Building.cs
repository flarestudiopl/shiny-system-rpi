using System.Collections.Generic;

namespace HeatingControl.Domain
{
    public class Building
    {
        public string Name { get; set; }

        public IList<Zone> Zones { get; set; }

        public int ControlLoopIntervalSeconds { get; set; }
    }
}
