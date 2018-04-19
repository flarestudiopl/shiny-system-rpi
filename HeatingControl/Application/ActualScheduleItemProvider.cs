using HeatingControl.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeatingControl.Application
{
    public interface IActualScheduleItemProvider
    {
        ScheduleItem TryProvide(ICollection<ScheduleItem> scheduleItems);
    }

    public class ActualScheduleItemProvider : IActualScheduleItemProvider
    {
        public ScheduleItem TryProvide(ICollection<ScheduleItem> scheduleItems)
        {
            var now = DateTime.Now;

            if (scheduleItems != null && scheduleItems.Any())
            {
                return scheduleItems.FirstOrDefault(x => x.DayOfWeek == now.DayOfWeek &&
                                               x.BeginTime.TimeOfDay > now.TimeOfDay &&
                                               x.EndTime.TimeOfDay <= now.TimeOfDay);
            }

            return null;
        }
    }
}
