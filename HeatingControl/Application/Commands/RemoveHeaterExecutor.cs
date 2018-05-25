using System.Linq;
using Commons;
using Commons.Extensions;
using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public interface IRemoveHeaterExecutor
    {
        void Execute(int heaterId, ControllerState controllerState, Building model);
    }

    public class RemoveHeaterExecutor : IRemoveHeaterExecutor
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public RemoveHeaterExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }
        public void Execute(int heaterId, ControllerState controllerState, Building model)
        {
            if (!controllerState.HeaterIdToState.ContainsKey(heaterId))
            {
                return;
            }

            if (controllerState.ZoneIdToState.Values.Any(x => x.Zone.HeaterIds.Contains(heaterId)))
            {
                Logger.Warning("Can't delete heater assigned to zone.");
                return;
            }

            controllerState.HeaterIdToState.Remove(heaterId);
            model.Heaters.Remove(x=>x.HeaterId == heaterId);
            _buildingModelSaver.Save(model);
        }
    }
}
