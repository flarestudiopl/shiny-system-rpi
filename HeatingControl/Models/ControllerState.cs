using HeatingControl.Domain;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HeatingControl.Models
{
    public class ControllerState
    {
        public ConcurrentDictionary<string, TemperatureData> DeviceIdToTemperatureData { get; set; } = new ConcurrentDictionary<string, TemperatureData>();

        public ConcurrentDictionary<string, TemperatureZoneState> TemperatureZoneNameToState { get; set; } = new ConcurrentDictionary<string, TemperatureZoneState>();

        public IDictionary<string, HeaterState> HeaterNameToState { get; set; } = new Dictionary<string, HeaterState>();

        public IList<PowerZoneState> PowerZoneNameToState { get; set; } = new List<PowerZoneState>();

        public IDictionary<PowerOutputDescriptor, bool> PowerOutputToState { get; set; } = new Dictionary<PowerOutputDescriptor, bool>();
    }
}
