using HardwareAccess.Devices;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HardwareAccess.DummyDevices
{
    public class TemperatureSensor : ITemperatureSensor
    {
        private readonly Random _random = new Random();

        public ICollection<string> GetAvailableSensors()
        {
            return new List<string> { "dummy-1", "dummy-2" };
        }

        public Task<TemperatureSensorData> Read(string deviceId)
        {
            return Task.Delay(750)
                .ContinueWith(_ => new TemperatureSensorData
                {
                    CrcOk = _random.Next(0, 100) < 95,
                    Value = (float)_random.NextDouble() + 20f
                });
        }
    }
}
