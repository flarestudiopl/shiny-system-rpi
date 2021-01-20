using Commons;
using Commons.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        const int RETRY_ATTEMPTS_COUNT = 3;
        const int RETRY_INTERVAL_MILLISECONDS = 250;

        private object _clientLock = new object();
        private readonly IDictionary<ModbusServerDescriptor, EasyModbus.ModbusClient> _modbusClientCache = new Dictionary<ModbusServerDescriptor, EasyModbus.ModbusClient>();

        public int ReadHoldingRegister(string ip, int port, int address)
        {
            try
            {
                return Try(() => TryGetConnectedClient(ip, port).ReadHoldingRegisters(address, 3)[0], RETRY_ATTEMPTS_COUNT, RETRY_INTERVAL_MILLISECONDS, nameof(ReadHoldingRegister));
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
                return Try(() => TryGetConnectedClient(ip, port)?.ReadInputRegisters(address, 3)[0] ?? int.MinValue, RETRY_ATTEMPTS_COUNT, RETRY_INTERVAL_MILLISECONDS, nameof(ReadInputRegister));
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
                Try(() => TryGetConnectedClient(ip, port)?.WriteSingleRegister(address, value), RETRY_ATTEMPTS_COUNT, RETRY_INTERVAL_MILLISECONDS, nameof(WriteHoldingRegister));
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
                    client.Disconnect();
                    _modbusClientCache.Remove(modbusDescriptor);

                    throw exception;
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
                    _modbusClientCache[modbusDescriptor].Disconnect();
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

        private void Try(Action action, int attempts, int millisecondsDelay, string operation)
        {
            Try<object>(() => { action(); return null; }, attempts, millisecondsDelay, operation);
        }

        private T Try<T>(Func<T> func, int attempts, int millisecondsDelay, string operation)
        {
            var attemptNumber = 0;
            Exception lastException = null;

            while (attemptNumber < attempts)
            {
                attemptNumber++;

                try
                {
                    Console.WriteLine($"ModbusTcp(Try) attempt no. {attemptNumber}/{attempts} for {operation}.");
                    return func();
                }
                catch (Exception e)
                {
                    lastException = e;
                }

                Task.Delay(millisecondsDelay).Wait();
            }

            throw lastException;
        }
    }
}
