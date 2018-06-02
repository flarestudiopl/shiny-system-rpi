using Commons.Extensions;
using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;
using System.Collections.Generic;
using System.Linq;

namespace HeatingControl.Application.Commands
{
    public interface IRemoveScheduleItemExecutor
    {
        void Execute(int zoneId, int scheduleItemId, ControllerState controllerState, Building model);
    }

    public class RemoveScheduleItemExecutor : IRemoveScheduleItemExecutor
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public RemoveScheduleItemExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public void Execute(int zoneId, int scheduleItemId, ControllerState controllerState, Building model)
        {
            var zone = controllerState.ZoneIdToState.GetValueOrDefault(zoneId);

            if (zone == null)
            {
                return;
            }

            var scheduleItem = zone.Zone.Schedule.FirstOrDefault(x => x.ScheduleItemId == scheduleItemId);

            if (scheduleItem == null)
            {
                return;
            }

            zone.Zone.Schedule.Remove(scheduleItem);

            _buildingModelSaver.Save(model);
        }
    }
}
