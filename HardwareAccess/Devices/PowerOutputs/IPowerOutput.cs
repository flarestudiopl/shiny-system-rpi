using System;

namespace HardwareAccess.Devices.PowerOutputs
{
    public interface IPowerOutput
    {
        string ProtocolName { get; }

        object ConfigurationOptions { get; }

        Type OutputDescriptorType { get; }

        void SetState(object outputDescriptor, bool state);

        bool GetState(object outputDescriptor);
    }
}
