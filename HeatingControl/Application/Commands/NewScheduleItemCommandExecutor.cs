using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Commons.Localization;
using Domain;
using HeatingControl.Application.DataAccess;
using HeatingControl.Extensions;

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
        private readonly IRepository<ScheduleItem> _scheduleItemRepository;

        public NewScheduleItemCommandExecutor(IRepository<ScheduleItem> scheduleItemRepository)
        {
            _scheduleItemRepository = scheduleItemRepository;
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

            var newScheduleItem = new ScheduleItem
                                  {
                                      ZoneId = zone.ZoneId,
                                      DaysOfWeek = command.DaysOfWeek,
                                      BeginTime = command.BeginTime,
                                      EndTime = command.EndTime,
                                  };

            if (zone.IsTemperatureControlled())
            {
                newScheduleItem.SetPoint = command.SetPoint;
            }

            _scheduleItemRepository.Create(newScheduleItem);
            zone.Schedule.Add(newScheduleItem);

            return new CommandResult();
        }
    }
}
