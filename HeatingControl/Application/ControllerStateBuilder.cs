using Commons;
using Commons.Extensions;
using HeatingControl.Domain;
using HeatingControl.Models;

namespace HeatingControl.Application
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

            foreach (var zone in buildingModel.TemperatureZones) // TODO model validation
            {
                if (zone.Name.IsNullOrEmpty())
                {
                    Logger.Warning("Skipping zone without name.");

                    continue;
                }

                if (!string.IsNullOrEmpty(zone.TemperatureSensorDeviceId))
                {
                    state.DeviceIdToTemperatureData.AddOrUpdate(zone.TemperatureSensorDeviceId, new TemperatureData(), (key, value) => value);
                }

                state.TemperatureZoneNameToState.AddOrUpdate(zone.Name, new TemperatureZoneState
                {
                    CurrentControlType = GetInitialControlType(zone),
                    TemperatureZone = zone
                }, (key, value) => value);

                foreach (var heater in zone.Heaters)
                {
                    if (heater.Name.IsNullOrEmpty())
                    {
                        Logger.Warning("Skipping heater without name.");

                        continue;
                    }

                    state.HeaterNameToState.Add(heater.Name, new HeaterState
                    {
                        Heater = heater,
                    });

                    state.PowerOutputToState.Add(heater.PowerOutput, false);
                }
            }

            return state;
        }

        private ControlType GetInitialControlType(TemperatureZone zone)
        {
            if (zone.AllowedControlTypes.HasFlag(ControlType.ScheduleOnOff))
            {
                return ControlType.ScheduleOnOff;
            }

            if (zone.AllowedControlTypes.HasFlag(ControlType.ScheduleTemperatureControl))
            {
                return ControlType.ScheduleTemperatureControl;
            }

            return ControlType.None;
        }
    }
}
