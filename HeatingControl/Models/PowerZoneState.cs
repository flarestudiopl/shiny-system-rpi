﻿using HeatingControl.Domain;
using System;
using System.Collections.Generic;

namespace HeatingControl.Models
{
    public class PowerZoneState
    {
        public PowerZone PowerZone { get; set; }

        public IDictionary<PowerOutput, bool> AffectedOutputToState { get; set; }

        public DateTime NextRoundDateTime { get; set; }
    }
}
