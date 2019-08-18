using HardwareAccess.Devices.DigitalInputs;
using System;
using System.Collections.Generic;

namespace HardwareAccess.Devices
{
    public interface IDigitalInputProvider
    {
        ICollection<string> GetAvailableProtocolNames();

        IDigitalInput Provide(string protocolName);
    }

    public class DigitalInputProvider : IDigitalInputProvider
    {
        private static readonly IDictionary<string, IDigitalInput> _availableDigitalInputs = new Dictionary<string, IDigitalInput>();

        public DigitalInputProvider(IShinyPowerState shinyPowerState)
        {
            _availableDigitalInputs.Add(shinyPowerState.ProtocolName, shinyPowerState);
        }

        public ICollection<string> GetAvailableProtocolNames()
        {
            return _availableDigitalInputs.Keys;
        }

        public IDigitalInput Provide(string protocolName)
        {
            if (!_availableDigitalInputs.TryGetValue(protocolName, out var digitalInput))
            {
                throw new ArgumentException(nameof(protocolName));
            }

            return digitalInput;
        }
    }
}
