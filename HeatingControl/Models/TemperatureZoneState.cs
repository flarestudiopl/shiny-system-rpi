using HeatingControl.Domain;

namespace HeatingControl.Models
{
    public class TemperatureZoneState
    {
        public TemperatureZone TemperatureZone { get; set; }

        public ControlType CurrentControlType { get; set; }

        public float SetPoint { get; set; }

        public bool EnableOutputs { get; set; }
    }
}
