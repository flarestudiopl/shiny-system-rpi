using HeatingControl.Domain;

namespace HeatingControl
{
    public interface IControllerStateBuilder
    {
        ControllerState Build(Building buildingModel);
    }

    public class ControllerStateBuilder : IControllerStateBuilder
    {
        public ControllerState Build(Building buildingModel)
        {
            var state = new ControllerState();

            foreach(var zone in buildingModel.TemperatureZones)
            {
                state.DeviceIdToTemperatureData.AddOrUpdate(zone.TemperatureSensorDeviceId, new ControllerState.TemperatureData(), (key, value) => value);
            }

            return state;
        }
    }
}
