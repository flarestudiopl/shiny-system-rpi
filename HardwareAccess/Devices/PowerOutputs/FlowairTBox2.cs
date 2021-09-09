using Commons;
using Domain;
using HardwareAccess.Buses;
using System;

namespace HardwareAccess.Devices.PowerOutputs
{
    public interface IFlowairTBox2 : IPowerOutput { }

    public class FlowairTBox2 : IFlowairTBox2
    {
        private struct OutputDescriptor
        {
            public string IpAddress { get; set; }
            public int PortNumber { get; set; }
        }

        private const int T_REF_ADDRESS = 0x06;

        private readonly IModbusTcp _modbusTcp;

        public string ProtocolName => ProtocolNames.FlowairTBox2;

        public object ConfigurationOptions => null;

        public Type OutputDescriptorType => typeof(OutputDescriptor);

        public FlowairTBox2(IModbusTcp modbusTcp)
        {
            _modbusTcp = modbusTcp;
        }

        public bool TrySetState(object outputDescriptor, bool state, float? setPoint)
        {
            var output = DescriptorHelper.CastHardwareDescriptorOrThrow<OutputDescriptor>(outputDescriptor);

            if (!setPoint.HasValue)
            {
                Logger.Warning("SetPoint is required for this type of output!");
                return false;
            }

            var tRefValue = (int)(setPoint.Value * 10f);
            var writeResult = _modbusTcp.WriteHoldingRegister(output.IpAddress, output.PortNumber, T_REF_ADDRESS, tRefValue);

            if (!writeResult.Success)
            {
                Logger.DebugWithData("Cannot write to output", writeResult.Exception?.ToString());
                return false;
            }

            return writeResult.Success;
        }

        public bool? TryGetState(object outputDescriptor)
        {
            return null;
        }
    }
}
