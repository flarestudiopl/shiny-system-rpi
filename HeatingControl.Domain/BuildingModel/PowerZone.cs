﻿using System.Collections.Generic;

namespace Domain.BuildingModel
{
    public  class PowerZone
    {
        public string Name { get; set; }
        
        public IList<PowerOutput> PowerOutputs { get; set; }

        public int RoundRobinIntervalMinutes { get; set; }
    }
}