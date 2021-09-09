using Commons;
using Domain;
using HardwareAccess.Buses;
using System;
using System.Threading.Tasks;

namespace HardwareAccess.Devices.TemperatureInputs
{
    public interface IFlowairTBox2 : ITemperatureInput { }

    public class FlowairTBox2 : IFlowairTBox2
    {
        private struct InputDescriptor
        {
            public string IpAddress { get; set; }
            public int PortNumber { get; set; }
            public byte InputRegisterAddress { get; set; }
            public double ValueMultiplier { get; set; }
            public double ValueOffset { get; set; }
        }

        private readonly IModbusTcp _modbusTcp;

        public string ProtocolName => ProtocolNames.FlowairTBox2;

        public object ConfigurationOptions => null;

        public Type InputDescriptorType => typeof(InputDescriptor);

        public FlowairTBox2(IModbusTcp modbusTcp)
        {
            _modbusTcp = modbusTcp;
        }

        public Task<TemperatureSensorData> GetValue(object inputDescriptor)
        {
            var input = DescriptorHelper.CastHardwareDescriptorOrThrow<InputDescriptor>(inputDescriptor);
            var temperatureReadout = _modbusTcp.ReadInputRegister(input.IpAddress, input.PortNumber, input.InputRegisterAddress);

            if (temperatureReadout.Exception != null)
            {
                Logger.DebugWithData("Modbus Exception", temperatureReadout.Exception.ToString());
            }

            return Task.FromResult(new TemperatureSensorData
            {
                Value = temperatureReadout.Value * input.ValueMultiplier + input.ValueOffset,
                Success = temperatureReadout.Success
            });
        }
    }
}
