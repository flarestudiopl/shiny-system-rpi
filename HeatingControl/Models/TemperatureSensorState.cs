using Domain;
using System;
using System.Collections.Generic;

namespace HeatingControl.Models
{
    public class TemperatureSensorState
    {
        public TemperatureSensor TemperatureSensor { get; set; }

        public double AverageTemperature { get; set; }

        public Queue<double> Readouts { get; set; } = new Queue<double>();

        public DateTime LastRead { get; set; }

        public Queue<Tuple<DateTime, double>> HistoricalReads { get; set; } = new Queue<Tuple<DateTime, double>>();
    }
}
