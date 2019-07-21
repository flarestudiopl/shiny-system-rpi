using Domain;
using HardwareAccess.Devices.PowerOutputs;
using System;
using System.Collections.Generic;

namespace HardwareAccess.Devices
{
    public interface IPowerOutputProvider
    {
        ICollection<string> GetAvailableProtocolNames();

        IPowerOutput Provide(string protocolName);
    }

    public class PowerOutputProvider : IPowerOutputProvider
    {
        private static readonly IDictionary<string, IPowerOutput> _availablePowerOutputs = new Dictionary<string, IPowerOutput>();

        public PowerOutputProvider(IInvertedPcfOutput invertedPcfOutput,
                                   IShinyMcpExpander shinyMcpExpander)
        {
            _availablePowerOutputs.Add(invertedPcfOutput.ProtocolName, invertedPcfOutput);
            _availablePowerOutputs.Add(shinyMcpExpander.ProtocolName, shinyMcpExpander);
        }

        public ICollection<string> GetAvailableProtocolNames()
        {
            return _availablePowerOutputs.Keys;
        }

        public IPowerOutput Provide(string protocolName)
        {
            if (!_availablePowerOutputs.TryGetValue(protocolName, out var powerOutput))
            {
                throw new ArgumentException(nameof(protocolName));
            }

            return powerOutput;
        }
    }
}
