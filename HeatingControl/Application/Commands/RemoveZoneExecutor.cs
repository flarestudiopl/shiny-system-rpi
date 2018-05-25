using Commons.Extensions;
using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public interface IRemoveZoneExecutor
    {
        void Execute(int zoneId, Building building, ControllerState controllerState);
    }

    public class RemoveZoneExecutor : IRemoveZoneExecutor
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public RemoveZoneExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public void Execute(int zoneId, Building building, ControllerState controllerState)
        {
            if (!controllerState.ZoneIdToState.ContainsKey(zoneId))
            {
                return;
            }

            var zoneState = controllerState.ZoneIdToState[zoneId];

            controllerState.ZoneIdToState.Remove(zoneId);

            foreach (var heaterToDisable in zoneState.Zone.HeaterIds)
            {
                controllerState.HeaterIdToState[heaterToDisable].OutputState = false;
            }

            building.Zones.Remove(x => x.ZoneId == zoneId);

            _buildingModelSaver.Save(building);
        }
    }
}
