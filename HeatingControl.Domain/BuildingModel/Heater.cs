namespace Domain.BuildingModel
{
    public class Heater
    {
        public int HeaterId { get; set; }

        public string Name { get; set; }

        public string PowerOutputProtocolName { get; set; }

        public int PowerOutputDeviceId { get; set; }

        public string PowerOutputChannel { get; set; }

        public UsageUnit UsageUnit { get; set; }

        public decimal UsagePerHour { get; set; }

        public int MinimumStateChangeIntervalSeconds { get; set; }
    }
}
