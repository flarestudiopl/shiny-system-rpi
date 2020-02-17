using System.Linq;
using Commons;
using Commons.Extensions;
using Commons.Localization;
using Domain;
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
            var state = new ControllerState
            {
                Model = buildingModel
            };

            MapConfiguredHeaters(buildingModel, state);
            MapConfiguredSensors(buildingModel, state);
            MapConfiguredZones(buildingModel, state);
            MapConfiguredPowerZones(buildingModel, state);
            MapConfiguredDigitalInputs(buildingModel, state);

            return state;
        }

        private static void MapConfiguredHeaters(Building buildingModel, ControllerState state)
        {
            if (buildingModel.Heaters == null)
            {
                return;
            }

            foreach (var heater in buildingModel.Heaters)
            {
                if (heater.Name.IsNullOrEmpty())
                {
                    Logger.Warning(Localization.NotificationMessage.SkippingHeaterWithoutName);

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
                    state.InstantUsage.Add(heater.UsageUnit, 0m);
                }
            }
        }

        private static void MapConfiguredSensors(Building buildingModel, ControllerState state)
        {
            if (buildingModel.TemperatureSensors == null)
            {
                return;
            }

            foreach (var sensor in buildingModel.TemperatureSensors)
            {
                if (sensor.Name.IsNullOrEmpty())
                {
                    Logger.Warning(Localization.NotificationMessage.SkippingSensorWithoutName);

                    continue;
                }

                state.TemperatureSensorIdToState.Add(sensor.TemperatureSensorId,
                                                     new TemperatureSensorState
                                                     {
                                                         TemperatureSensor = sensor
                                                     });
            }
        }

        private void MapConfiguredZones(Building buildingModel, ControllerState state)
        {
            if (buildingModel.Zones == null)
            {
                return;
            }

            foreach (var zone in buildingModel.Zones)
            {
                if (zone.Name.IsNullOrEmpty())
                {
                    Logger.Warning(Localization.NotificationMessage.SkippingZoneWithoutName);

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
            if (buildingModel.PowerZones == null)
            {
                return;
            }

            foreach (var powerZone in buildingModel.PowerZones)
            {
                if (powerZone.Name.IsNullOrEmpty())
                {
                    Logger.Warning(Localization.NotificationMessage.SkippingPowerZoneWithoutName);

                    continue;
                }

                state.PowerZoneIdToState.Add(powerZone.PowerZoneId,
                                             new PowerZoneState
                                             {
                                                 PowerZone = powerZone
                                             });
            }
        }

        private void MapConfiguredDigitalInputs(Building buildingModel, ControllerState state)
        {
            if (buildingModel.DigitalInputs == null)
            {
                return;
            }

            state.DigitalInputFunctionToState = buildingModel.DigitalInputs.ToDictionary(x => x.Function,
                                                                                         x => new DigitalInputState
                                                                                         {
                                                                                             DigitalInput = x
                                                                                         });
        }

        private static ZoneControlMode GetInitialControlMode(Zone zone)
        {
            return zone.Schedule.Count > 0 ? ZoneControlMode.Schedule : ZoneControlMode.LowOrDisabled;
        }
    }
}
