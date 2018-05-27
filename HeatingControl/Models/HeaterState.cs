﻿using Domain.BuildingModel;
using System;

namespace HeatingControl.Models
{
    public class HeaterState
    {
        public Heater Heater { get; set; }

        public DateTime LastStateChange { get; set; }

        public DateTime LastCounterStart { get; set; }

        public bool OutputState { get; set; }
    }
}
