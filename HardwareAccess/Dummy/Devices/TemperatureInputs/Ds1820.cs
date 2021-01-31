using Domain;
using HardwareAccess.Devices.TemperatureInputs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HardwareAccess.Dummy.Devices.TemperatureInputs
{
    public class Ds1820 : IDs1820
    {
        private readonly Random _random = new Random();

        public string ProtocolName => ProtocolNames.Ds1820;

        public object ConfigurationOptions => new { AvailableSensors = new  List<string> { "10-0008019e9d54", "28-000005964edc", "28-000005964aaa", "28-000005964bbb", "28-000005964ccc" } };

        public Type InputDescriptorType => typeof(HardwareAccess.Devices.TemperatureInputs.Ds1820.InputDescriptor);

        public Task<TemperatureSensorData> GetValue(object inputDescriptor)
        {
            return Task.Delay(750)
                       .ContinueWith(_ => new TemperatureSensorData
                       {
                           Success = _random.Next(0, 100) < 98,
                           Value = _random.NextDouble() + 20d
                       });
        }
    }
}
