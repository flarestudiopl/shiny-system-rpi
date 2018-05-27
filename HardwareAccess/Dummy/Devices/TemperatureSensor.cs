using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HardwareAccess.Devices;

namespace HardwareAccess.Dummy.Devices
{
    public class TemperatureSensor : ITemperatureSensor
    {
        private readonly Random _random = new Random();

        public ICollection<string> GetAvailableSensors()
        {
            return new List<string> { "10-0008019e9d54", "28-000005964edc" };
        }

        public Task<TemperatureSensorData> Read(string deviceId)
        {
            return Task.Delay(750)
                .ContinueWith(_ => new TemperatureSensorData
                {
                    CrcOk = _random.Next(0, 100) < 98,
                    Value = (float)_random.NextDouble() + 20f
                });
        }
    }
}
