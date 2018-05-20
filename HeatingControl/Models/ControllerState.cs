using System.Collections.Generic;
using Domain.BuildingModel;

namespace HeatingControl.Models
{
    public class ControllerState
    {
        public IDictionary<string, TemperatureData> TemperatureDeviceIdToTemperatureData { get; set; } = new Dictionary<string, TemperatureData>();

        public IDictionary<int, string> TemperatureSensorIdToDeviceId { get; set; } = new Dictionary<int, string>();

        public IDictionary<int, ZoneState> ZoneIdToState { get; set; } = new Dictionary<int, ZoneState>();

        public IDictionary<int, HeaterState> HeaterIdToState { get; set; } = new Dictionary<int, HeaterState>();

        public IDictionary<UsageUnit, float> InstantUsage { get; set; } = new Dictionary<UsageUnit, float>();

        public IDictionary<int, PowerZoneState> PowerZoneIdToState { get; set; } = new Dictionary<int, PowerZoneState>();
    }
}
