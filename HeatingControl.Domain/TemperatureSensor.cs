using System.Collections.Generic;

namespace Domain
{
    public class TemperatureSensor
    {
        public int TemperatureSensorId { get; set; }

        public int BuildingId { get; set; }
        
        public string Name { get; set; }

        public string DeviceId { get; set; }

        public ICollection<TemperatureControlledZone> TemperatureControlledZones { get; set; }
    }
}
