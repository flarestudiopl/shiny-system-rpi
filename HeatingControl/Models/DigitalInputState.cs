using Domain.BuildingModel;
using System;

namespace HeatingControl.Models
{
    public class DigitalInputState
    {
        public DigitalInput DigitalInput { get; set; }

        public bool State { get; set; }

        public DateTime LastRead { get; set; }
    }
}
