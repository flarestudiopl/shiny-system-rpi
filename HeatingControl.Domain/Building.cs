using System.Collections.Generic;

namespace Domain
{
    public class Building
    {
        public int BuildingId { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public int ControlLoopIntervalSecondsMilliseconds { get; set; }

        public ICollection<Zone> Zones { get; set; }

        public ICollection<Heater> Heaters { get; set; }

        public ICollection<TemperatureSensor> TemperatureSensors { get; set; }

        public ICollection<PowerZone> PowerZones { get; set; }

        public ICollection<DigitalInput> DigitalInputs { get; set; }
    }
}
