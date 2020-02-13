using Commons;
using System;

namespace HardwareAccess.Devices
{
    public static class DescriptorHelper
    {
        public static T CastHardwareDescriptorOrThrow<T>(object descriptor)
        {
            if (descriptor is T)
            {
                return (T)descriptor;
            }

            Logger.DebugWithData("Unable to cast hw descriptor.", descriptor);
            throw new ArgumentException("Descriptor-protocol mismatch.");
        }
    }
}
