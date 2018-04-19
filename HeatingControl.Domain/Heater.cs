namespace HeatingControl.Domain
{
    public class Heater
    {
        public int HeaterId { get; set; }

        public string Name { get; set; }

        public PowerOutput PowerOutput { get; set; }

        public UsageUnit UsageUnit { get; set; }

        public float UsagePerHour { get; set; }

        public int MinimumStateChangeIntervalSeconds { get; set; }
    }
}
