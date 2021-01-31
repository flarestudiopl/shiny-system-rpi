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
        const int RETRY_ATTEMPTS_COUNT = 4;
        const int RETRY_INTERVAL_MILLISECONDS = 100;

        private object _clientLock = new object();
        private readonly IDictionary<ModbusServerDescriptor, EasyModbus.ModbusClient> _modbusClientCache = new Dictionary<ModbusServerDescriptor, EasyModbus.ModbusClient>();

        public int ReadHoldingRegister(string ip, int port, int address)
        {
            const byte p = 3;

            try
            {
                return Try(() => TryGetConnectedClient(ip, port, p).ReadHoldingRegisters(address, 3)[0], RETRY_ATTEMPTS_COUNT, RETRY_INTERVAL_MILLISECONDS, nameof(ReadHoldingRegister) + ip);
            }
            catch (Exception exception)
            {
                Logger.Error("ModbusTcp read holding register error", exception);
                RemoveDeadClient(ip, port, p);
                return int.MinValue;
            }
        }

        public int ReadInputRegister(string ip, int port, int address)
        {
            const byte p = 2;

            try
            {
                return Try(() => TryGetConnectedClient(ip, port, p)?.ReadInputRegisters(address, 3)[0] ?? int.MinValue, RETRY_ATTEMPTS_COUNT, RETRY_INTERVAL_MILLISECONDS, nameof(ReadInputRegister) + ip);
            }
            catch (Exception exception)
            {
                Logger.Error("ModbusTcp read input register error", exception);
                RemoveDeadClient(ip, port, p);
                return int.MinValue;
            }
        }

        public void WriteHoldingRegister(string ip, int port, int address, int value)
        {
            const byte p = 1;
            try
            {
                Try(() => TryGetConnectedClient(ip, port, p)?.WriteSingleRegister(address, value), RETRY_ATTEMPTS_COUNT, RETRY_INTERVAL_MILLISECONDS, nameof(WriteHoldingRegister) + ip);
            }
            catch (Exception exception)
            {
                Logger.Error("ModbusTcp write holding register error", exception);
                RemoveDeadClient(ip, port, p);
            }
        }

        private EasyModbus.ModbusClient TryGetConnectedClient(string ip, int port, byte p)
        {
            lock (_clientLock)
            {
                var modbusDescriptor = ModbusServerDescriptor.Create(ip, port, p);
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

        private void RemoveDeadClient(string ip, int port, byte p)
        {
            lock (_clientLock)
            {
                var modbusDescriptor = ModbusServerDescriptor.Create(ip, port, p);

                if (_modbusClientCache.TryGetValue(modbusDescriptor, out var client))
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
            public byte Purpose { get; set; }
            public static ModbusServerDescriptor Create(string ip, int port, byte purpose) => new ModbusServerDescriptor { IpAddress = ip, PortNumber = port, Purpose = purpose };
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
                Task.Delay(millisecondsDelay).Wait();

                try
                {
                    return func();
                }
                catch (Exception e)
                {
                    lastException = e;
                }
            }

            Console.WriteLine($"ModbusTcp(Try) failed after {attemptNumber} attempts for {operation}.");

            throw lastException;
        }
    }
}
