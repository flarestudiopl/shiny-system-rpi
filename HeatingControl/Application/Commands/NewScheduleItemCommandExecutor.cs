using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Domain.BuildingModel;
using HeatingControl.Extensions;
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

        public CommandResult Execute(NewScheduleItemCommand command, CommandContext context)
        {
            if (command.BeginTime >= command.EndTime)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.EndTimeShouldBeGreaterThanBeginTime);
            }

            if (command.DaysOfWeek == null || !command.DaysOfWeek.Any())
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.MissingDaysOfWeek);
            }

            var zone = context.ControllerState.Model.Zones.FirstOrDefault(x => x.ZoneId == command.ZoneId);

            if (zone == null)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.UnknownZoneId.FormatWith(command.ZoneId));
            }

            if (zone.IsTemperatureControlled() && !command.SetPoint.HasValue)
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.SetPointIsForTemperatureControlledZoneOnly);
            }

            if (zone.Schedule.Any(x => x.DaysOfWeek.Any(d => command.DaysOfWeek.Contains(d) &&
                                                             (command.BeginTime >= x.BeginTime && command.BeginTime < x.EndTime ||
                                                              command.EndTime > x.BeginTime && command.EndTime <= x.EndTime ||
                                                              command.BeginTime < x.BeginTime && command.EndTime > x.EndTime))))
            {
                return CommandResult.WithValidationError(Localization.ValidationMessage.ScheduleItemOverlaps);
            }

            var lastScheduleItem = zone.Schedule.OrderByDescending(x => x.ScheduleItemId).FirstOrDefault();

            var newScheduleItem = new ScheduleItem
                                  {
                                      ScheduleItemId = (lastScheduleItem?.ScheduleItemId ?? 0) + 1,
                                      DaysOfWeek = command.DaysOfWeek,
                                      BeginTime = command.BeginTime,
                                      EndTime = command.EndTime,
                                      SetPoint = command.SetPoint
                                  };

            zone.Schedule.Add(newScheduleItem);

            _buildingModelSaver.Save(context.ControllerState.Model);

            return new CommandResult();
        }
    }
}
