using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public interface ISaveZoneExecutor
    {
        void Execute(SaveZoneExecutorInput input, Building building, ControllerState state);
    }

    public class SaveZoneExecutorInput
    {
        public int ZoneId { get; set; }
        // TODO
    }

    public class SaveZoneExecutor : ISaveZoneExecutor
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public SaveZoneExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public void Execute(SaveZoneExecutorInput input, Building building, ControllerState state)
        {
            // TODO - add or create zone in model

            _buildingModelSaver.Save(building);

            // TODO - add zone to state
        }
    }
}
