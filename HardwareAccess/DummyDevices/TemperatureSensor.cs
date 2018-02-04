using HardwareAccess.Devices;
using System;
using System.Threading.Tasks;

namespace HardwareAccess.DummyDevices
{
    public class TemperatureSensor : ITemperatureSensor
    {
        private readonly Random _random = new Random();

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
