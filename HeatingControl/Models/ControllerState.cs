using HeatingControl.Domain;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HeatingControl.Models
{
    public class ControllerState
    {
        public ConcurrentDictionary<string, TemperatureData> DeviceIdToTemperatureData { get; set; } = new ConcurrentDictionary<string, TemperatureData>();

        public IDictionary<string, string> TemperatureSensorNameToDeviceId { get; set; } = new Dictionary<string, string>();

        public ConcurrentDictionary<string, ZoneState> ZoneNameToState { get; set; } = new ConcurrentDictionary<string, ZoneState>();

        public IDictionary<string, HeaterState> HeaterNameToState { get; set; } = new Dictionary<string, HeaterState>();

        // TODO: power limits
        //public IList<PowerZoneState> PowerZoneNameToState { get; set; } = new List<PowerZoneState>();

        public IDictionary<PowerOutput, bool> PowerOutputToState { get; set; } = new Dictionary<PowerOutput, bool>();
    }
}
