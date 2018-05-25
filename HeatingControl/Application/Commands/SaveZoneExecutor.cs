using System.Collections.Generic;
using System.Linq;
using Commons;
using Commons.Extensions;
using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public interface ISaveZoneExecutor
    {
        void Execute(SaveZoneExecutorInput input, Building building, ControllerState controllerState);
    }

    public class SaveZoneExecutorInput
    {
        public int? ZoneId { get; set; }
        public string Name { get; set; }
        public int? TemperatureSensorId { get; set; }
        public ICollection<int> HeaterIds { get; set; }
    }

    public class SaveZoneExecutor : ISaveZoneExecutor
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public SaveZoneExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public void Execute(SaveZoneExecutorInput input, Building building, ControllerState controllerState)
        {
            if (!InputIsValid(input, controllerState))
            {
                return;
            }

            Zone existingZone = null;

            if (input.ZoneId.HasValue)
            {
                var existingZoneId = input.ZoneId.Value;
                var zoneState = controllerState.ZoneIdToState[existingZoneId];
                existingZone = zoneState.Zone;

                controllerState.ZoneIdToState.Remove(existingZoneId);

                foreach (var heaterToDisable in zoneState.Zone
                                                         .HeaterIds
                                                         .Except(input.HeaterIds))
                {
                    controllerState.HeaterIdToState[heaterToDisable].OutputState = false;
                }

                building.Zones.Remove(x => x.ZoneId == existingZoneId);
            }

            var zone = BuildNewZone(input, existingZone, controllerState);

            building.Zones.Add(zone);

            _buildingModelSaver.Save(building);

            controllerState.ZoneIdToState.Add(zone.ZoneId,
                                              new ZoneState
                                              {
                                                  Zone = zone,
                                                  ControlMode = ZoneControlMode.LowOrDisabled,
                                                  EnableOutputs = false
                                              });
        }

        private static bool InputIsValid(SaveZoneExecutorInput input, ControllerState controllerState)
        {
            if (input.Name.IsNullOrEmpty())
            {
                Logger.Warning("Cannot add zone without name.");
                return false;
            }

            if (input.ZoneId.HasValue && !controllerState.ZoneIdToState.ContainsKey(input.ZoneId.Value))
            {
                Logger.Warning("Unknown zone id {0}.", new object[] { input.ZoneId.Value });
                return false;
            }

            foreach (var heaterId in input.HeaterIds)
            {
                if (!controllerState.HeaterIdToState.ContainsKey(heaterId))
                {
                    Logger.Warning("Heater of id {0} not found.", new object[] { heaterId });
                    return false;
                }

                if (controllerState.ZoneIdToState
                                   .Select(z => z.Value.Zone)
                                   .Where(x => !input.ZoneId.HasValue || input.ZoneId.Value != x.ZoneId)
                                   .Any(z => z.HeaterIds.Contains(heaterId)))
                {
                    Logger.Warning("Heater of id {0} is already in use by another zone.", new object[] { heaterId });
                    return false;
                }
            }

            return true;
        }

        private static Zone BuildNewZone(SaveZoneExecutorInput input, Zone existingZone, ControllerState controllerState)
        {
            var zone = new Zone
                       {
                           Name = input.Name,
                           HeaterIds = input.HeaterIds.ToHashSet()
                       };

            if (input.TemperatureSensorId.HasValue)
            {
                if (existingZone?.TemperatureControlledZone != null)
                {
                    zone.TemperatureControlledZone = existingZone.TemperatureControlledZone;
                }
                else
                {
                    zone.TemperatureControlledZone = new TemperatureControlledZone
                                                     {
                                                         LowSetPoint = 4.0f,
                                                         HighSetPoint = 20.0f,
                                                         Hysteresis = 0.5f,
                                                         ScheduleDefaultSetPoint = 4.0f
                                                     };
                }

                zone.TemperatureControlledZone.TemperatureSensorId = input.TemperatureSensorId.Value;
            }

            if (existingZone != null)
            {
                zone.Schedule = existingZone.Schedule;
                zone.ZoneId = existingZone.ZoneId;
            }
            else
            {
                zone.ZoneId = (controllerState.ZoneIdToState.Keys.Any() ? controllerState.ZoneIdToState.Keys.Max() : 0) + 1;
            }

            return zone;
        }
    }
}
