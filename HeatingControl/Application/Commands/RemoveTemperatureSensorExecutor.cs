using System.Linq;
using Commons;
using Commons.Extensions;
using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public interface IRemoveTemperatureSensorExecutor
    {
        void Execute(int sensorId, ControllerState controllerState, Building model);
    }

    public class RemoveTemperatureSensorExecutor : IRemoveTemperatureSensorExecutor
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public RemoveTemperatureSensorExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public void Execute(int sensorId, ControllerState controllerState, Building model)
        {
            if (!controllerState.TemperatureSensorIdToDeviceId.ContainsKey(sensorId))
            {
                return;
            }

            if (model.Zones.Any(x => x.TemperatureControlledZone?.TemperatureSensorId == sensorId))
            {
                Logger.Warning("Can't delete sensor assigned to zone.");
                return;
            }

            controllerState.TemperatureSensorIdToDeviceId.Remove(sensorId);
            model.TemperatureSensors.Remove(x => x.TemperatureSensorId == sensorId);

            _buildingModelSaver.Save(model);
        }
    }
}
