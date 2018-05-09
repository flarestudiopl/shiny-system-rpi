using System;

namespace HeatingControl.Domain
{
    public class ScheduleItem
    {
        public int ScheduleItemId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan BeginTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public float? SetPoint { get; set; }
    }
}
