using System;

namespace HeatingControl.Domain
{
    [Flags]
    public enum ControlType
    {
        None = 0,

        ManualOnOff = 1 << 0,
        ScheduleOnOff = 1 << 1,

        ManualTemperatureControl = 1 << 2,
        ScheduleTemperatureControl = 1 << 3,
    }
}
