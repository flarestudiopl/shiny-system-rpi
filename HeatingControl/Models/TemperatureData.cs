using System;
using System.Collections.Generic;

namespace HeatingControl.Models
{
    public class TemperatureData
    {
        public float AverageTemperature { get; set; }

        public Queue<float> Readouts { get; set; } = new Queue<float>();

        public DateTime LastRead { get; set; }
    }
}
