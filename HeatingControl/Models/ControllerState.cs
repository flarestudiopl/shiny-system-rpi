using System.Collections.Concurrent;
using System.Collections.Generic;
using HeatingControl.Domain;

namespace HeatingControl.Models
{
    public class ControllerState
    {
        public ConcurrentDictionary<string, TemperatureData> TemperatureDeviceIdToTemperatureData { get; set; } = new ConcurrentDictionary<string, TemperatureData>();

        public IDictionary<int, string> TemperatureSensorIdToDeviceId { get; set; } = new Dictionary<int, string>();

        public ConcurrentDictionary<int, ZoneState> ZoneIdToState { get; set; } = new ConcurrentDictionary<int, ZoneState>();

        public IDictionary<int, HeaterState> HeaterIdToState { get; set; } = new Dictionary<int, HeaterState>();

        public IDictionary<UsageUnit, float> InstantUsage { get; set; } = new Dictionary<UsageUnit, float>();

        // TODO: power limits
        //public IList<PowerZoneState> PowerZoneNameToState { get; set; } = new List<PowerZoneState>();
    }
}
