using System.Collections.Generic;

namespace Domain.BuildingModel
{
    public class Building
    {
        public string Name { get; set; }

        public ICollection<Zone> Zones { get; set; }

        public ICollection<Heater> Heaters { get; set; }

        public ICollection<TemperatureSensor> TemperatureSensors { get; set; }

        public ICollection<PowerZone> PowerZones { get; set; }

        public int ControlLoopIntervalSecondsMilliseconds { get; set; }
    }
}
