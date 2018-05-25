using Commons.Extensions;
using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public interface IRemovePowerZoneExecutor
    {
        void Execute(int powerZoneId, ControllerState controllerState, Building model);
    }

    public class RemovePowerZoneExecutor : IRemovePowerZoneExecutor
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public RemovePowerZoneExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public void Execute(int powerZoneId, ControllerState controllerState, Building model)
        {
            if (!controllerState.PowerZoneIdToState.ContainsKey(powerZoneId))
            {
                return;
            }

            controllerState.PowerZoneIdToState.Remove(powerZoneId);
            model.PowerZones.Remove(x => x.PowerZoneId == powerZoneId);

            _buildingModelSaver.Save(model);
        }
    }
}
