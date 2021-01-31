using Domain;
using System;

namespace HeatingControl.Models
{
    public class HeaterState
    {
        public Heater Heater { get; set; }

        public DateTime? StateChangeFailureSince { get; set; }

        public DateTime LastStateChange { get; set; }

        public DateTime LastCounterStart { get; set; } = DateTime.UtcNow;

        public bool OutputState { get; set; }
    }
}
