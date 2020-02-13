using HardwareAccess.Devices.TemperatureInputs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HardwareAccess.Devices
{
    public interface ITemperatureInputProvider
    {
        ICollection<string> GetAvailableProtocolNames();
        TemperatureInputWrapper Provide(string protocolName);
    }

    public class TemperatureInputWrapper
    {
        private static readonly object _inputDescriptorCacheLock = new object();
        private static readonly IDictionary<string, object> _inputDescriptorCache = new Dictionary<string, object>();

        private readonly ITemperatureInput _temperatureInput;

        public TemperatureInputWrapper(ITemperatureInput temperatureInput)
        {
            _temperatureInput = temperatureInput;
        }

        public string ProtocolName => _temperatureInput.ProtocolName;
        public object ConfigurationOptions => _temperatureInput.ConfigurationOptions;
        public Type InputDescriptorType => _temperatureInput.InputDescriptorType;

        public Task<TemperatureSensorData> GetValue(string inputDescriptorJson)
        {
            var input = GetInputDescriptor(inputDescriptorJson);
            return _temperatureInput.GetValue(input);
        }

        private object GetInputDescriptor(string inputDescriptorJson)
        {
            lock (_inputDescriptorCacheLock)
            {
                if (!_inputDescriptorCache.TryGetValue(inputDescriptorJson, out var inputDescriptor))
                {
                    inputDescriptor = Newtonsoft.Json.JsonConvert.DeserializeObject(inputDescriptorJson, InputDescriptorType);
                    _inputDescriptorCache.Add(inputDescriptorJson, inputDescriptor);
                }

                return inputDescriptor;
            }
        }
    }

    public class TemperatureInputProvider : ITemperatureInputProvider
    {
        private static readonly IDictionary<string, ITemperatureInput> _availableTemperatureInputs = new Dictionary<string, ITemperatureInput>();

        public TemperatureInputProvider(IDs1820 ds1820)
        {
            _availableTemperatureInputs.Add(ds1820.ProtocolName, ds1820);
        }

        public ICollection<string> GetAvailableProtocolNames()
        {
            return _availableTemperatureInputs.Keys;
        }

        public TemperatureInputWrapper Provide(string protocolName)
        {
            if (!_availableTemperatureInputs.TryGetValue(protocolName, out var temperatureInput))
            {
                throw new ArgumentException(nameof(protocolName));
            }

            return new TemperatureInputWrapper(temperatureInput);
        }
    }
}
