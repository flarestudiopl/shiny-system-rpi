using System.Collections.Generic;
using HeatingControl.Domain;
using HeatingControl.Models;

namespace HeatingControl.Application.Queries
{
    public interface IZoneDetailsProvider
    {
        ZoneDetailsProviderResult Provide(int zoneId, ControllerState controllerState);
    }

    public class ZoneDetailsProviderResult
    {
        public CountersData Counters { get; set; }

        public TemperatureSettings Temperatures { get; set; }

        public ICollection<ScheduleItem> Schedule { get; set; }

        public ConfigurationData Configuration { get; set; }

        public class CountersData
        {

        }

        public class TemperatureSettings
        {
            public float LowSetPoint { get; set; }
            public float HightSetPoint { get; set; }
            public float ScheduleDefaultSetPoint { get; set; }
            public float Hysteresis { get; set; }
        }

        public class ConfigurationData
        {
            public string Name { get; set; }

            public int? TemperatureSensorId { get; set; }

            public ICollection<SensorData> TemperatureSensors { get; set; }

            public ICollection<int> HeaterIds { get; set; }

            public ICollection<HeaterData> Heaters { get; set; }

            public class SensorData
            {
                public int Id { get; set; }

                public string Name { get; set; }

                public float Readout { get; set; }
            }

            public class HeaterData
            {
                public int Id { get; set; }

                public string Name { get; set; }

                public string Assignment { get; set; }
            }
        }
    }

    public class ZoneDetailsProvider : IZoneDetailsProvider
    {
        public ZoneDetailsProviderResult Provide(int zoneId, ControllerState controllerState)
        {
            throw new System.NotImplementedException();
        }
    }
}
