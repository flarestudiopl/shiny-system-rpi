using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Domain.BuildingModel;
using HeatingControl.Extensions;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
   public class NewScheduleItemCommand
    {
        public int ZoneId { get; set; }
        public ICollection<DayOfWeek> DaysOfWeek { get; set; }
        public TimeSpan BeginTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public float? SetPoint { get; set; }
    }

    public class NewScheduleItemCommandExecutor : ICommandExecutor<NewScheduleItemCommand>
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public NewScheduleItemCommandExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public CommandResult Execute(NewScheduleItemCommand command, ControllerState controllerState)
        {
            var input = command; // TODO
            var building = controllerState.Model;

            if (input.BeginTime >= input.EndTime)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.EndTimeShouldBeGreaterThanBeginTime);
            }

            if (input.DaysOfWeek == null || !input.DaysOfWeek.Any())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.MissingDaysOfWeek);
            }

            var zone = building.Zones.FirstOrDefault(x => x.ZoneId == input.ZoneId);

            if (zone == null)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownZoneId.FormatWith(input.ZoneId));
            }

            if (zone.IsTemperatureControlled() && !input.SetPoint.HasValue)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.SetPointIsForTemperatureControlledZoneOnly);
            }

            if (zone.Schedule.Any(x => x.DaysOfWeek.Any(d => input.DaysOfWeek.Contains(d) &&
                                                             (input.BeginTime >= x.BeginTime && input.BeginTime < x.EndTime ||
                                                              input.EndTime > x.BeginTime && input.EndTime <= x.EndTime ||
                                                              input.BeginTime < x.BeginTime && input.EndTime > x.EndTime))))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.ScheduleItemOverlaps);
            }

            var lastScheduleItem = zone.Schedule.OrderByDescending(x => x.ScheduleItemId).FirstOrDefault();

            var newScheduleItem = new ScheduleItem
                                  {
                                      ScheduleItemId = (lastScheduleItem?.ScheduleItemId ?? 0) + 1,
                                      DaysOfWeek = input.DaysOfWeek,
                                      BeginTime = input.BeginTime,
                                      EndTime = input.EndTime,
                                      SetPoint = input.SetPoint
                                  };

            zone.Schedule.Add(newScheduleItem);

            _buildingModelSaver.Save(building);

            return new CommandResult();
        }
    }
}
