using System;
using System.Collections.Generic;
using Domain;

namespace HeatingControl.Models
{
    public class ControllerState
    {
        public bool ControlEnabled { get; set; }

        public Building Model { get; set; }

        public IDictionary<int, TemperatureSensorState> TemperatureSensorIdToState { get; set; } = new Dictionary<int, TemperatureSensorState>();

        public IDictionary<int, ZoneState> ZoneIdToState { get; set; } = new Dictionary<int, ZoneState>();

        public IDictionary<int, HeaterState> HeaterIdToState { get; set; } = new Dictionary<int, HeaterState>();

        public IDictionary<UsageUnit, decimal> InstantUsage { get; set; } = new Dictionary<UsageUnit, decimal>();

        public IDictionary<int, PowerZoneState> PowerZoneIdToState { get; set; } = new Dictionary<int, PowerZoneState>();

        public IDictionary<DigitalInputFunction, DigitalInputState> DigitalInputFunctionToState { get; set; } = new Dictionary<DigitalInputFunction, DigitalInputState>();

        public DateTime? ScheduledShutdownUtcTime { get; set; }

        public bool ShutdownRequested { get; set; }
    }
}
