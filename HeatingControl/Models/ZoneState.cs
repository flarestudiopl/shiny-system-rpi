using HeatingControl.Domain;

namespace HeatingControl.Models
{
    public class ZoneState
    {
        public Zone Zone { get; set; }

        public ZoneControlMode ControlMode { get; set; }

        public bool EnableOutputs { get; set; }
    }
}
