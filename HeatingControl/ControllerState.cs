using HardwareAccess.Devices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace HeatingControl
{
    public class ControllerState
    {
        public ConcurrentDictionary<string, TemperatureData> DeviceIdToTemperatureData { get; set; } = new ConcurrentDictionary<string, TemperatureData>();
    }

    public class TemperatureData
    {
        public float AverageTemperature { get; set; }

        public Queue<TemperatureSensorData> Readouts { get; set; } = new Queue<TemperatureSensorData>();

        public DateTime LastRead { get; set; }
    }
}
