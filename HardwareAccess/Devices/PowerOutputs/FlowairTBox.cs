using Commons;
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

        private const int WORK_MODE_ADDRESS = 0x04; // TODO - CHECK IN DOCS

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

        public bool TrySetState(object outputDescriptor, bool state, float? setPoint)
        {
            var output = DescriptorHelper.CastHardwareDescriptorOrThrow<OutputDescriptor>(outputDescriptor);
            var bmsState = _modbusTcp.ReadHoldingRegister(output.IpAddress, output.PortNumber, BMS_MODE_ADDRESS);

            if (!bmsState.Success)
            {
                Logger.DebugWithData("Cannot read BMS state", bmsState.Exception?.ToString());
                return false;
            }

            if (bmsState.Value != BMS_WM_RAW)
            {
                var writeBmsState = _modbusTcp.WriteHoldingRegister(output.IpAddress, output.PortNumber, BMS_MODE_ADDRESS, BMS_WM_RAW);
                if (!writeBmsState.Success)
                {
                    Logger.DebugWithData("Cannot write BMS state", writeBmsState.Exception?.ToString());
                    return false;
                }
            }

            var registerAddress = WORK_MODE_ADDRESS + GetOffset(output.DriverAddress);
            var writeResult = _modbusTcp.WriteHoldingRegister(output.IpAddress, output.PortNumber, registerAddress, state ? 2 : 1); // TODO - check states & move to const

            if (!writeResult.Success)
            {
                Logger.DebugWithData("Cannot write to output", writeResult.Exception?.ToString());
                return false;
            }

            return writeResult.Success;
        }

        public bool? TryGetState(object outputDescriptor)
        {
            var output = DescriptorHelper.CastHardwareDescriptorOrThrow<OutputDescriptor>(outputDescriptor);
            var registerAddress = WORK_MODE_ADDRESS + GetOffset(output.DriverAddress);
            var registerRead = _modbusTcp.ReadHoldingRegister(output.IpAddress, output.PortNumber, registerAddress);

            if (registerRead.Success)
            {
                return registerRead.Value != 1;
            }

            if (registerRead.Exception != null)
            {
                Logger.DebugWithData("Register read exception", registerRead.Exception.ToString());
            }

            return null;
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
