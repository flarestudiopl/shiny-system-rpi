using System.Collections.Generic;

namespace HeatingControl.Domain
{
    public class Building
    {
        public string Name { get; set; }

        public ICollection<Zone> Zones { get; set; }

        public ICollection<Heater> Heaters { get; set; }

        public ICollection<TemperatureSensor> TemperatureSensors { get; set; }


        // TODO: power limits 
        //public IList<PowerZone> PowerZones { get; set; }

        public int ControlLoopIntervalSecondsMilliseconds { get; set; }
    }
}
