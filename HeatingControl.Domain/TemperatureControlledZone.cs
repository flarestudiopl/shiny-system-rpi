namespace HeatingControl.Domain
{
    public class TemperatureControlledZone
    {
        public string TemperatureSensorName { get; set; }

        public float LowSetPoint { get; set; }

        public float HighSetPoint { get; set; }

        public float ScheduleDefaultSetPoint { get; set; }

        public float Hysteresis { get; set; }

    }
}
