using System.Collections.Generic;

namespace HeatingControl.Domain
{
    public  class PowerZone
    {
        public string Name { get; set; }
        
        public IList<PowerOutputDescriptor> PowerOutputs { get; set; }

        public int RoundRobinIntervalMinutes { get; set; }
    }
}
