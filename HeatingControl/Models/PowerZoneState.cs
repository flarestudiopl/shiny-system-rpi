using Domain.BuildingModel;
using System;

namespace HeatingControl.Models
{
    public class PowerZoneState
    {
        public PowerZone PowerZone { get; set; }

        public DateTime NextIntervalIncrementationDateTime { get; set; }

        public byte NextIntervalOffset { get; set; }
    }
}
