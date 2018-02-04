using HeatingControl.Domain;
using System;

namespace HeatingControl.Models
{
    public class HeaterState
    {
        public Heater Heater { get; set; }

        public DateTime LastStateChange { get; set; }

        public bool OutputState { get; set; }
    }
}
