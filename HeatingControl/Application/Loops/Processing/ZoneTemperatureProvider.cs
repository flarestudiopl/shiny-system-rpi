using Commons.Extensions;
using HeatingControl.Models;

namespace HeatingControl.Application.Loops.Processing
{
    public interface IZoneTemperatureProvider
    {
        TemperatureSensorState Provide(int zoneId, ControllerState state);
    }

    public class ZoneTemperatureProvider : IZoneTemperatureProvider
    {
        public TemperatureSensorState Provide(int zoneId, ControllerState state)
        {
            var zone = state.ZoneIdToState[zoneId].Zone;
            var temperatureSensorState = state.TemperatureSensorIdToState.GetValueOrDefault(zone.TemperatureControlledZone.TemperatureSensorId);

            return temperatureSensorState;
        }
    }
}
