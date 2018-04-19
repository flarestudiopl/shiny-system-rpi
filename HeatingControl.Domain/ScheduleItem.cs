using System;

namespace HeatingControl.Domain
{
    public class ScheduleItem
    {
        public int ScheduleItemId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        public float? SetPoint { get; set; }
    }
}
