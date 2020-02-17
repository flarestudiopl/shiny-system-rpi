using Domain;
using HardwareAccess.Buses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HardwareAccess.Devices.TemperatureInputs
{
    public interface IFlowairTBox : ITemperatureInput { }

    public class FlowairTBox : IFlowairTBox
    {
        private struct InputDescriptor
        {
            public string IpAddress { get; set; }
            public int PortNumber { get; set; }
            public byte DriverAddress { get; set; }
            public byte InputRegisterAddress { get; set; }
            public double ValueMultiplier { get; set; }
            public double ValueOffset { get; set; }
        }

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

        public Type InputDescriptorType => typeof(InputDescriptor);

        public FlowairTBox(IModbusTcp modbusTcp)
        {
            _modbusTcp = modbusTcp;
        }

        public Task<TemperatureSensorData> GetValue(object inputDescriptor)
        {
            var input = DescriptorHelper.CastHardwareDescriptorOrThrow<InputDescriptor>(inputDescriptor);
            var temperatureReadout = _modbusTcp.ReadInputRegister(input.IpAddress, input.PortNumber, GetOffset(input.DriverAddress) + input.InputRegisterAddress);

            return Task.FromResult(new TemperatureSensorData
            {
                Value = temperatureReadout * input.ValueMultiplier + input.ValueOffset,
                CrcOk = temperatureReadout != 1024 && temperatureReadout != int.MinValue
            });
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
