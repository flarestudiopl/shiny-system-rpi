using Domain;
using HardwareAccess.Buses;
using System;
using System.Collections.Generic;

namespace HardwareAccess.Devices.PowerOutputs
{
    public interface IFlowairTBox : IPowerOutput { }

    public class FlowairTBox : IFlowairTBox
    {
        private struct OutputDescriptor
        {
            public string IpAddress { get; set; }
            public int PortNumber { get; set; }
            public byte DriverAddress { get; set; }
        }

        private const int BMS_MODE_ADDRESS = 0x04;
        private const int BMS_WM_RAW = 0x0001;

        private const int WORK_MODE_ADDRESS = 0x04;

        private readonly static IDictionary<byte, int> DRIVER_ADDRESS_TO_REGISTER_OFFSET = new Dictionary<byte, int>
        {
            [1] = 0x0100,
            [2] = 0x0140,
            [3] = 0x0180,
            [4] = 0x01C0
        };

        private readonly IModbusTcp _modbusTcp;

        public string ProtocolName => ProtocolNames.FlowairTBoxR;

        public object ConfigurationOptions => new { Drivers = DRIVER_ADDRESS_TO_REGISTER_OFFSET.Keys };

        public Type OutputDescriptorType => typeof(OutputDescriptor);

        public FlowairTBox(IModbusTcp modbusTcp)
        {
            _modbusTcp = modbusTcp;
        }

        public void SetState(object outputDescriptor, bool state)
        {
            var output = DescriptorHelper.CastHardwareDescriptorOrThrow<OutputDescriptor>(outputDescriptor);
            var bmsState = _modbusTcp.ReadHoldingRegister(output.IpAddress, output.PortNumber, BMS_MODE_ADDRESS);

            if(bmsState != BMS_WM_RAW)
            {
                _modbusTcp.WriteHoldingRegister(output.IpAddress, output.PortNumber, BMS_MODE_ADDRESS, BMS_WM_RAW);
            }

            var registerAddress = WORK_MODE_ADDRESS + GetOffset(output.DriverAddress);

            _modbusTcp.WriteHoldingRegister(output.IpAddress, output.PortNumber, registerAddress, state ? 2 : 1);
        }

        public bool GetState(object outputDescriptor)
        {
            var output = DescriptorHelper.CastHardwareDescriptorOrThrow<OutputDescriptor>(outputDescriptor);
            var registerAddress = WORK_MODE_ADDRESS + GetOffset(output.DriverAddress);
            var registerValue = _modbusTcp.ReadHoldingRegister(output.IpAddress, output.PortNumber, registerAddress);

            return registerValue != 1;
        }

        private int GetOffset(byte driverAddress)
        {
            if (!DRIVER_ADDRESS_TO_REGISTER_OFFSET.TryGetValue(driverAddress, out var offset))
            {
                throw new ArgumentException(nameof(driverAddress));
            }

            return offset;
        }
    }
}
