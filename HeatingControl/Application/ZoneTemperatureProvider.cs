using HeatingControl.Models;

namespace HeatingControl.Application
{
    public interface IZoneTemperatureProvider
    {
        TemperatureData Provide(int zoneId, ControllerState state);
    }

    public class ZoneTemperatureProvider : IZoneTemperatureProvider
    {
        public TemperatureData Provide(int zoneId, ControllerState state)
        {
            var zone = state.ZoneIdToState[zoneId].Zone;
            var temperatureSensorDeviceId = state.TemperatureSensorIdToDeviceId[zone.TemperatureControlledZone.TemperatureSensorId];

            state.TemperatureDeviceIdToTemperatureData.TryGetValue(temperatureSensorDeviceId, out var temperatureData);

            return temperatureData;
        }
    }
}
