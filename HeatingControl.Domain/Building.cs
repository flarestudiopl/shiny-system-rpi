using System.Collections.Generic;

namespace Domain
{
    public class Building
    {
        public int BuildingId { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public int ControlLoopIntervalSecondsMilliseconds { get; set; }

        public ICollection<Zone> Zones { get; set; } = new List<Zone>();

        public ICollection<Heater> Heaters { get; set; } = new List<Heater>();

        public ICollection<TemperatureSensor> TemperatureSensors { get; set; } = new List<TemperatureSensor>();

        public ICollection<PowerZone> PowerZones { get; set; } = new List<PowerZone>();

        public ICollection<DigitalInput> DigitalInputs { get; set; } = new List<DigitalInput>();
    }
}
