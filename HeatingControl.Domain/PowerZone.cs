using System.Collections.Generic;

namespace Domain
{
    public  class PowerZone
    {
        public int PowerZoneId { get; set; }

        public int BuildingId { get; set; }
        
        public string Name { get; set; }

        public decimal MaxUsage { get; set; }

        public UsageUnit UsageUnit { get; set; }

        public int RoundRobinIntervalMinutes { get; set; }
        
        public ICollection<Heater> Heaters { get; set; }
    }
}
