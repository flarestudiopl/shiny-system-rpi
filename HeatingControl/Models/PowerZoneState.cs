using Domain;
using System;

namespace HeatingControl.Models
{
    public class PowerZoneState
    {
        public PowerZone PowerZone { get; set; }

        public DateTime NextIntervalIncrementationDateTime { get; set; }

        public byte NextIntervalOffset { get; set; }

        public DateTime LastOutputStateChange { get; set; }
    }
}
