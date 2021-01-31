using HardwareAccess.Devices.PowerOutputs;
using System;
using System.Collections.Generic;

namespace HardwareAccess.Devices
{
    public interface IPowerOutputProvider
    {
        ICollection<string> GetAvailableProtocolNames();

        PowerOutputWrapper Provide(string protocolName);
    }

    public class PowerOutputWrapper
    {
        private static readonly object _outputDescriptorCacheLock = new object();
        private static readonly IDictionary<string, object> _outputDescriptorCache = new Dictionary<string, object>();

        private readonly IPowerOutput _powerOutput;

        public PowerOutputWrapper(IPowerOutput powerOutput)
        {
            _powerOutput = powerOutput;
        }

        public string ProtocolName => _powerOutput.ProtocolName;
        public object ConfigurationOptions => _powerOutput.ConfigurationOptions;
        public Type OutputDescriptorType => _powerOutput.OutputDescriptorType;

        public bool? TryGetState(string outputDescriptorJson)
        {
            var outputDescriptor = GetOutputDescriptor(outputDescriptorJson);
            return _powerOutput.TryGetState(outputDescriptor);
        }

        public bool TrySetState(string outputDescriptorJson, bool state)
        {
            var outputDescriptor = GetOutputDescriptor(outputDescriptorJson);
            return _powerOutput.TrySetState(outputDescriptor, state);
        }

        private object GetOutputDescriptor(string outputDescriptorJson)
        {
            lock (_outputDescriptorCacheLock)
            {
                if (!_outputDescriptorCache.TryGetValue(outputDescriptorJson, out var outputDescriptor))
                {
                    outputDescriptor = Newtonsoft.Json.JsonConvert.DeserializeObject(outputDescriptorJson, OutputDescriptorType);
                    _outputDescriptorCache.Add(outputDescriptorJson, outputDescriptor);
                }

                return outputDescriptor;
            }
        }
    }

    public class PowerOutputProvider : IPowerOutputProvider
    {
        private static readonly IDictionary<string, IPowerOutput> _availablePowerOutputs = new Dictionary<string, IPowerOutput>();

        public PowerOutputProvider(IInvertedPcfOutput invertedPcfOutput,
                                   IShinyMcpExpander shinyMcpExpander,
                                   IFlowairTBox flowairTBox)
        {
            _availablePowerOutputs.Add(invertedPcfOutput.ProtocolName, invertedPcfOutput);
            _availablePowerOutputs.Add(shinyMcpExpander.ProtocolName, shinyMcpExpander);
            _availablePowerOutputs.Add(flowairTBox.ProtocolName, flowairTBox);
        }

        public ICollection<string> GetAvailableProtocolNames()
        {
            return _availablePowerOutputs.Keys;
        }

        public PowerOutputWrapper Provide(string protocolName)
        {
            if (!_availablePowerOutputs.TryGetValue(protocolName, out var powerOutput))
            {
                throw new ArgumentException(nameof(protocolName));
            }

            return new PowerOutputWrapper(powerOutput);
        }
    }
}
