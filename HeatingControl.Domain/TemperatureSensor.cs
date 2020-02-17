using System.Collections.Generic;

namespace Domain
{
    public class TemperatureSensor
    {
        public int TemperatureSensorId { get; set; }

        public int BuildingId { get; set; }

        public string Name { get; set; }

        public string ProtocolName { get; set; }

        public string InputDescriptor { get; set; }

        public ICollection<TemperatureControlledZone> TemperatureControlledZones { get; set; }
    }
}
