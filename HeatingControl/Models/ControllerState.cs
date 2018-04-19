using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HeatingControl.Models
{
    public class ControllerState
    {
        public ConcurrentDictionary<string, TemperatureData> DeviceIdToTemperatureData { get; set; } = new ConcurrentDictionary<string, TemperatureData>();

        public IDictionary<int, string> TemperatureSensorIdToDeviceId { get; set; } = new Dictionary<int, string>();

        public ConcurrentDictionary<int, ZoneState> ZoneIdToState { get; set; } = new ConcurrentDictionary<int, ZoneState>();

        public IDictionary<int, HeaterState> HeaterIdToState { get; set; } = new Dictionary<int, HeaterState>();

        // TODO: power limits
        //public IList<PowerZoneState> PowerZoneNameToState { get; set; } = new List<PowerZoneState>();
    }
}
