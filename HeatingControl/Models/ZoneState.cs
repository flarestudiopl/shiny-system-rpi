using Domain;
using System;

namespace HeatingControl.Models
{
    public class ZoneState
    {
        public Zone Zone { get; set; }

        public ZoneControlMode ControlMode { get; set; }

        public bool EnableOutputs { get; set; }

        public ScheduleState ScheduleState { get; set; } = new ScheduleState();
    }

    public class ScheduleState
    {
        public bool? HeatingEnabled { get; set; }

        public float? DesiredTemperature { get; set; }
    } 
}
