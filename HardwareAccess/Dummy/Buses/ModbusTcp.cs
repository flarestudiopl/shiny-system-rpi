using HardwareAccess.Buses;
using System;
using System.Collections.Generic;

namespace HardwareAccess.Dummy.Buses
{
    public class ModbusTcp : IModbusTcp
    {
        private readonly IDictionary<FakeRegisterDescriptor, int> _inputRegisters = new Dictionary<FakeRegisterDescriptor, int>();
        private readonly Random _random = new Random();

        public int ReadInputRegister(string ip, int port, int address)
        {
            return _random.Next(150, 180);

        }

        public int ReadHoldingRegister(string ip, int port, int address)
        {
            var registerDescriptor = new FakeRegisterDescriptor { IpAddress = ip, PortNumber = port, Address = address };

            return _inputRegisters.ContainsKey(registerDescriptor) ? _inputRegisters[registerDescriptor] : 0;
        }

        public void WriteHoldingRegister(string ip, int port, int address, int value)
        {
            var registerDescriptor = new FakeRegisterDescriptor { IpAddress = ip, PortNumber = port, Address = address };

            _inputRegisters[registerDescriptor] = value;
        }

        private struct FakeRegisterDescriptor
        {
            public string IpAddress { get; set; }
            public int PortNumber { get; set; }
            public int Address { get; set; }
        }
    }
}
