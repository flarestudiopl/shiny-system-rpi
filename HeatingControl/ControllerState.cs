using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HeatingControl
{
    public class ControllerState
    {
        public ConcurrentDictionary<string, TemperatureData> DeviceIdToTemperatureData { get; set; } = new ConcurrentDictionary<string, TemperatureData>();
    }

    public class TemperatureData
    {
        public float AverageTemperature { get; set; }

        public Queue<float> Readouts { get; set; } = new Queue<float>();

        public DateTime LastRead { get; set; }
    }
}
