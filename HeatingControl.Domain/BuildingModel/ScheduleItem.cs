using System;
using System.Collections.Generic;

namespace Domain.BuildingModel
{
    public class ScheduleItem
    {
        public int ScheduleItemId { get; set; }

        public ICollection<DayOfWeek> DaysOfWeek { get; set; }

        public TimeSpan BeginTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public float? SetPoint { get; set; }
    }
}
