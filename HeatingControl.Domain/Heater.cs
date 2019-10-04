namespace Domain
{
    public class Heater
    {
        public int HeaterId { get; set; }
        
        public int BuildingId { get; set; }
        
        public string Name { get; set; }

        public int DigitalOutputId { get;set;}

        public DigitalOutput DigitalOutput { get; set; }

        public UsageUnit UsageUnit { get; set; }

        public decimal UsagePerHour { get; set; }

        public int MinimumStateChangeIntervalSeconds { get; set; }

        public int? ZoneId { get; set; }

        public Zone Zone { get; set; }

        public int? PowerZoneId { get; set; }

        public PowerZone PowerZone { get; set; }
    }
}
