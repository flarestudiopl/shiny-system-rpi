using HardwareAccess.Buses;
using System;
using System.Collections.Generic;

namespace HardwareAccess.Dummy.Buses
{
    public class ModbusTcp : IModbusTcp
    {
        private readonly IDictionary<FakeRegisterDescriptor, int> _inputRegisters = new Dictionary<FakeRegisterDescriptor, int>();
        private readonly Random _random = new Random();

        public ModbusTcpReadResult ReadInputRegister(string ip, int port, int address)
        {
            return new ModbusTcpReadResult { Success = true, Value = _random.Next(150, 180) };

        }

        public ModbusTcpReadResult ReadHoldingRegister(string ip, int port, int address)
        {
            var registerDescriptor = new FakeRegisterDescriptor { IpAddress = ip, PortNumber = port, Address = address };
            var result = _inputRegisters.ContainsKey(registerDescriptor) ? _inputRegisters[registerDescriptor] : 0;

            return new ModbusTcpReadResult { Success = true, Value = result };
        }

        public ModbusTcpWriteResult WriteHoldingRegister(string ip, int port, int address, int value)
        {
            var registerDescriptor = new FakeRegisterDescriptor { IpAddress = ip, PortNumber = port, Address = address };

            _inputRegisters[registerDescriptor] = value;

            return new ModbusTcpWriteResult { Success = true };
        }

        private struct FakeRegisterDescriptor
        {
            public string IpAddress { get; set; }
            public int PortNumber { get; set; }
            public int Address { get; set; }
        }
    }
}
