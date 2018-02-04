using System.Collections.Generic;

namespace HeatingControl.Domain
{
    public class TemperatureZone
    {
        public string Name { get; set; }

        public string TemperatureSensorDeviceId { get; set; }

        public IList<Heater> Heaters { get; set; }

        public ControlType AllowedControlTypes { get; set; }

        public float DefaultSetPoint { get; set; }

        public float ManualSetPoint { get; set; }

        public float Hysteresis { get; set; }

        public IList<ScheduleItem> Schedule { get; set; }
    }
}
