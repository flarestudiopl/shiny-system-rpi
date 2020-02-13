using Commons;
using Commons.Extensions;
using System;
using System.Collections.Generic;

namespace HardwareAccess.Buses
{
    public interface IModbusTcp
    {
        int ReadInputRegister(string ip, int port, int address);
        int ReadHoldingRegister(string ip, int port, int address);
        void WriteHoldingRegister(string ip, int port, int address, int value);
    }

    public class ModbusTcp : IModbusTcp
    {
        private object _clientLock = new object();
        private readonly IDictionary<ModbusServerDescriptor, EasyModbus.ModbusClient> _modbusClientCache = new Dictionary<ModbusServerDescriptor, EasyModbus.ModbusClient>();

        public int ReadHoldingRegister(string ip, int port, int address)
        {
            try
            {
                return TryGetConnectedClient(ip, port)?.ReadHoldingRegisters(address, 1)[0] ?? int.MinValue;
            }
            catch (Exception exception)
            {
                Logger.Error("ModbusTcp read holding register error", exception);
                RemoveDeadClient(ip, port);
                return int.MinValue;
            }
        }

        public int ReadInputRegister(string ip, int port, int address)
        {
            try
            {
                return TryGetConnectedClient(ip, port)?.ReadInputRegisters(address, 1)[0] ?? int.MinValue;
            }
            catch (Exception exception)
            {
                Logger.Error("ModbusTcp read input register error", exception);
                RemoveDeadClient(ip, port);
                return int.MinValue;
            }
        }

        public void WriteHoldingRegister(string ip, int port, int address, int value)
        {
            try
            {
                TryGetConnectedClient(ip, port)?.WriteSingleRegister(address, value);
            }
            catch (Exception exception)
            {
                Logger.Error("ModbusTcpwrite holding register error", exception);
                RemoveDeadClient(ip, port);
            }
        }

        private EasyModbus.ModbusClient TryGetConnectedClient(string ip, int port)
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

                try
                {
                    if (!client.Connected)
                    {
                        client.Connect();
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error("ModbusTcp connection error", exception);
                    _modbusClientCache.Remove(modbusDescriptor);
                    return null;
                }

                return client;
            }
        }

        private void RemoveDeadClient(string ip, int port)
        {
            lock (_clientLock)
            {
                var modbusDescriptor = ModbusServerDescriptor.Create(ip, port);

                if (_modbusClientCache.ContainsKey(modbusDescriptor))
                {
                    _modbusClientCache.Remove(modbusDescriptor);
                }
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
