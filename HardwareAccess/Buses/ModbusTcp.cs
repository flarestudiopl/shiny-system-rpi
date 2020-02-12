using Commons.Extensions;
using System.Collections.Generic;

namespace HardwareAccess.Buses
{
    public interface IModbusTcp
    {
        int ReadHoldingRegister(string ip, int port, int address);
        void WriteHoldingRegister(string ip, int port, int address, int value);
    }

    public class ModbusTcp : IModbusTcp
    {
        private object _clientLock = new object();
        private readonly IDictionary<ModbusServerDescriptor, EasyModbus.ModbusClient> _modbusClientCache = new Dictionary<ModbusServerDescriptor, EasyModbus.ModbusClient>();

        public int ReadHoldingRegister(string ip, int port, int address)
        {
            return GetConnectedClient(ip, port).ReadHoldingRegisters(address, 1)[0];
        }

        public void WriteHoldingRegister(string ip, int port, int address, int value)
        {
            GetConnectedClient(ip, port).WriteSingleRegister(address, value);
        }

        private EasyModbus.ModbusClient GetConnectedClient(string ip, int port)
        {
            lock (_clientLock)
            {
                var modbusDescriptor = ModbusServerDescriptor.Create(ip, port);
                var client = _modbusClientCache.GetValueOrDefault(modbusDescriptor);

                if (client == null)
                {
                    client = new EasyModbus.ModbusClient(ip, port);
                    _modbusClientCache.Add(modbusDescriptor, client);
                }

                if (!client.Connected)
                {
                    client.Connect();
                }

                return client;
            }
        }

        private struct ModbusServerDescriptor
        {
            public string IpAddress { get; set; }
            public int PortNumber { get; set; }
            public static ModbusServerDescriptor Create(string ip, int port) => new ModbusServerDescriptor { IpAddress = ip, PortNumber = port };
        }
    }
}
