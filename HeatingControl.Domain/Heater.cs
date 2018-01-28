namespace HeatingControl.Domain
{
    public class Heater
    {
        public string Name { get; set; }

        public int PowerOutputDeviceId { get; set; }

        public int PowerOutputChannel { get; set; }

        public UsageUnit UsageUnit { get; set; }

        public float UsagePerHour { get; set; }

        public int MinimumStateChangeIntervalSeconds { get; set; }
    }
}
