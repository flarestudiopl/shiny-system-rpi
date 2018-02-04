namespace HeatingControl.Domain
{
    public class Heater
    {
        public string Name { get; set; }

        public PowerOutputDescriptor PowerOutput { get; set; }

        public UsageUnit UsageUnit { get; set; }

        public float UsagePerHour { get; set; }

        public int MinimumStateChangeIntervalSeconds { get; set; }
    }
}
