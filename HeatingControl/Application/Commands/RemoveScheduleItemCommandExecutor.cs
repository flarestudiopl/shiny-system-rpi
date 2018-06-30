﻿using Commons.Extensions;
using HeatingControl.Models;
using Storage.BuildingModel;
using System.Linq;
using Commons.Localization;

namespace HeatingControl.Application.Commands
{
    public class RemoveScheduleItemCommand
    {
        public int ZoneId { get; set; }
        public int ScheduleItemId { get; set; }
    }

    public class RemoveScheduleItemCommandExecutor : ICommandExecutor<RemoveScheduleItemCommand>
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public RemoveScheduleItemCommandExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public CommandResult Execute(RemoveScheduleItemCommand command, ControllerState controllerState)
        {
            var zone = controllerState.ZoneIdToState.GetValueOrDefault(command.ZoneId);

            if (zone == null)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownZoneId.FormatWith(command.ZoneId));
            }

            var scheduleItem = zone.Zone.Schedule.FirstOrDefault(x => x.ScheduleItemId == command.ScheduleItemId);

            if (scheduleItem == null)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownScheduleItemId.FormatWith(command.ZoneId));
            }

            zone.Zone.Schedule.Remove(scheduleItem);

            _buildingModelSaver.Save(controllerState.Model);

            return CommandResult.Empty;
        }
    }
}
