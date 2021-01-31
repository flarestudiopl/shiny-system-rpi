using System;
using System.Threading.Tasks;

namespace HardwareAccess.Devices.TemperatureInputs
{
    public interface ITemperatureInput
    {
        string ProtocolName { get; }

        object ConfigurationOptions { get; }

        Type InputDescriptorType { get; }

        Task<TemperatureSensorData> GetValue(object inputDescriptor);
    }

    public struct TemperatureSensorData
    {
        public double Value { get; set; }

        public bool Success { get; set; }
    }
}
