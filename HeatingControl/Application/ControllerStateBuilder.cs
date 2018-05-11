using Commons;
using Commons.Extensions;
using HardwareAccess.Devices;
using Domain.BuildingModel;
using HeatingControl.Models;

namespace HeatingControl.Application
{
    public interface IControllerStateBuilder
    {
        ControllerState Build(Building buildingModel);
    }

    public class ControllerStateBuilder : IControllerStateBuilder
    {
        private readonly ITemperatureSensor _temperatureSensor;

        public ControllerStateBuilder(ITemperatureSensor temperatureSensor)
        {
            _temperatureSensor = temperatureSensor;
        }

        public ControllerState Build(Building buildingModel)
        {
            var state = new ControllerState();

            foreach (var sensor in _temperatureSensor.GetAvailableSensors())
            {
                state.TemperatureDeviceIdToTemperatureData.AddOrUpdate(sensor, new TemperatureData(), (key, value) => value);
            }

            foreach (var heater in buildingModel.Heaters)
            {
                if (heater.Name.IsNullOrEmpty())
                {
                    Logger.Warning("Skipping heater without name.");

                    continue;
                }

                state.HeaterIdToState.Add(heater.HeaterId,
                                          new HeaterState
                                          {
                                              Heater = heater,
                                              OutputState = false
                                          });

                if (!state.InstantUsage.ContainsKey(heater.UsageUnit))
                {
                    state.InstantUsage.Add(heater.UsageUnit, 0f);
                }
            }

            foreach (var sensor in buildingModel.TemperatureSensors)
            {
                if (sensor.Name.IsNullOrEmpty())
                {
                    Logger.Warning("Skipping sensor without name.");

                    continue;
                }

                state.TemperatureSensorIdToDeviceId.Add(sensor.TemperatureSensorId, sensor.DeviceId);
            }

            foreach (var zone in buildingModel.Zones) // TODO model validation
            {
                if (zone.Name.IsNullOrEmpty())
                {
                    Logger.Warning("Skipping zone without name.");

                    continue;
                }

                state.ZoneIdToState.AddOrUpdate(zone.ZoneId,
                                                new ZoneState
                                                {
                                                    Zone = zone,
                                                    ControlMode = GetInitialControlMode(zone),
                                                    EnableOutputs = false
                                                },
                                                (key, value) => value);
            }

            return state;
        }

        private ZoneControlMode GetInitialControlMode(Zone zone)
        {
            return zone.Schedule.Count > 0 ? ZoneControlMode.Schedule : ZoneControlMode.LowOrDisabled;
        }
    }
}
