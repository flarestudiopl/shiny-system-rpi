using System.Collections.Generic;

namespace Domain.BuildingModel
{
    public  class PowerZone
    {
        public int PowerZoneId { get; set; }

        public string Name { get; set; }

        public HashSet<int> HeaterIds { get; set; } = new HashSet<int>();

        public float MaxUsage { get; set; }

        public UsageUnit UsageUnit { get; set; }

        public int RoundRobinIntervalMinutes { get; set; }
    }
}
