using System.Linq;
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

            CollectAvailableSensors(state);
            MapConfiguredHeaters(buildingModel, state);
            MapConfiguredSensors(buildingModel, state);
            MapConfiguredZones(buildingModel, state);
            MapConfiguredPowerZones(buildingModel, state);

            return state;
        }

        private void CollectAvailableSensors(ControllerState state)
        {
            foreach (var sensor in _temperatureSensor.GetAvailableSensors())
            {
                state.TemperatureDeviceIdToTemperatureData.Add(sensor, new TemperatureData());
            }
        }

        private static void MapConfiguredHeaters(Building buildingModel, ControllerState state)
        {
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
        }

        private static void MapConfiguredSensors(Building buildingModel, ControllerState state)
        {
            foreach (var sensor in buildingModel.TemperatureSensors)
            {
                if (sensor.Name.IsNullOrEmpty())
                {
                    Logger.Warning("Skipping sensor without name.");

                    continue;
                }

                state.TemperatureSensorIdToDeviceId.Add(sensor.TemperatureSensorId, sensor.DeviceId);
            }
        }

        private void MapConfiguredZones(Building buildingModel, ControllerState state)
        {
            foreach (var zone in buildingModel.Zones)
            {
                if (zone.Name.IsNullOrEmpty())
                {
                    Logger.Warning("Skipping zone without name.");

                    continue;
                }

                state.ZoneIdToState.Add(zone.ZoneId,
                                                new ZoneState
                                                {
                                                    Zone = zone,
                                                    ControlMode = GetInitialControlMode(zone),
                                                    EnableOutputs = false
                                                });
            }
        }

        private static void MapConfiguredPowerZones(Building buildingModel, ControllerState state)
        {
            foreach (var powerZone in buildingModel.PowerZones)
            {
                if (powerZone.Name.IsNullOrEmpty())
                {
                    Logger.Warning("Skipping power zone without name.");

                    continue;
                }

                state.PowerZoneIdToState.Add(powerZone.PowerZoneId,
                                             new PowerZoneState
                                             {
                                                 PowerZone = powerZone
                                             });
            }
        }

        private static ZoneControlMode GetInitialControlMode(Zone zone)
        {
            return zone.Schedule.Count > 0 ? ZoneControlMode.Schedule : ZoneControlMode.LowOrDisabled;
        }
    }
}
