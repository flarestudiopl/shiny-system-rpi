using Domain.BuildingModel;
using System;
using System.Collections.Generic;

namespace HeatingControl.Models
{
    public class PowerZoneState
    {
        public PowerZone PowerZone { get; set; }

        public IDictionary<int, bool> HeaterIdToPowerOnAllowance { get; set; }

        public DateTime NextAllowanceRecalculationDateTime { get; set; }

        public int NextAllowanceRecalculationOffset { get; set; }
    }
}
