using HeatingControl.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HeatingControl
{
    public class ControllerState
    {
        public ConcurrentDictionary<string, TemperatureData> DeviceIdToTemperatureData { get; set; } = new ConcurrentDictionary<string, TemperatureData>();

        public ConcurrentDictionary<string, TemperatureZoneState> TemperatureZoneNameToState { get; set; } = new ConcurrentDictionary<string, TemperatureZoneState>();

        public IDictionary<string, HeaterState> HeaterNameToState { get; set; } = new Dictionary<string, HeaterState>();

        public IList<PowerZoneState> PowerZoneNameToState { get; set; } = new List<PowerZoneState>();

        public ConcurrentDictionary<PowerOutputDescriptor, bool> PowerOutputToState { get; set; } = new ConcurrentDictionary<PowerOutputDescriptor, bool>();

        public class TemperatureData
        {
            public float AverageTemperature { get; set; }

            public Queue<float> Readouts { get; set; } = new Queue<float>();

            public DateTime LastRead { get; set; }
        }

        public class TemperatureZoneState
        {
            public TemperatureZone TemperatureZone { get; set; }

            public ControlType CurrentControlType { get; set; }

            public float SetPoint { get; set; }

            public bool EnableOutputs { get; set; }
        }

        public class HeaterState
        {
            public Heater Heater { get; set; }

            public DateTime LastStateChange { get; set; }

            public bool OutputState { get; set; }
        }

        public class PowerZoneState
        {
            public PowerZone PowerZone { get; set; }

            public IDictionary<PowerOutputDescriptor, bool> AffectedOutputToState { get; set; }

            public DateTime NextRoundDateTime { get; set; }
        }
    }
}
