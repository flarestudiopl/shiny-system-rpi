using System;

namespace HardwareAccess.Devices.PowerOutputs
{
    public interface IPowerOutput
    {
        string ProtocolName { get; }

        object ConfigurationOptions { get; }

        Type OutputDescriptorType { get; }

        bool TrySetState(object outputDescriptor, bool state);

        bool? TryGetState(object outputDescriptor);
    }
}
