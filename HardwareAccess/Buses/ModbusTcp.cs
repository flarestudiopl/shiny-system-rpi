using Commons;
using Commons.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HardwareAccess.Buses
{
    public interface IModbusTcp
    {
        ModbusTcpReadResult ReadInputRegister(string ip, int port, int address);
        ModbusTcpReadResult ReadHoldingRegister(string ip, int port, int address);
        ModbusTcpWriteResult WriteHoldingRegister(string ip, int port, int address, int value);
    }

    public class ModbusTcpReadResult
    {
        public int Value { get; set; }
        public bool Success { get; set; }
        public Exception Exception { get; set; }
    }

    public class ModbusTcpWriteResult
    {
        public bool Success { get; set; }
        public Exception Exception { get; set; }
    }

    public class ModbusTcp : IModbusTcp
    {
        const int RETRY_ATTEMPTS_COUNT = 5;
        const int RETRY_INTERVAL_MILLISECONDS = 100;

        private object _clientLock = new object();
        private readonly IDictionary<ModbusServerDescriptor, EasyModbus.ModbusClient> _modbusClientCache = new Dictionary<ModbusServerDescriptor, EasyModbus.ModbusClient>();

        public ModbusTcpReadResult ReadHoldingRegister(string ip, int port, int address)
        {
            const byte CLIENT_PURPOSE = 3;

            var getClientResult = TryGetConnectedClient(ip, port, CLIENT_PURPOSE);

            if (getClientResult.Exception != null)
            {
                return new ModbusTcpReadResult { Success = false, Exception = getClientResult.Exception };
            }

            if (getClientResult.Client == null)
            {
                return new ModbusTcpReadResult { Success = false, Exception = new Exception("Client cannot be null.") };
            }

            lock (getClientResult.Client)
            {
                var tryResult = Try(() => getClientResult.Client.ReadHoldingRegisters(address, 3)[0], RETRY_ATTEMPTS_COUNT, RETRY_INTERVAL_MILLISECONDS);

                if (tryResult.Exception != null)
                {
                    Console.WriteLine($"ModbusTcp - Read HR {address}@{ip}:{port} failed.");
                    RemoveDeadClient(ip, port, CLIENT_PURPOSE);

                    return new ModbusTcpReadResult { Success = false, Exception = tryResult.Exception };
                }

                return new ModbusTcpReadResult { Success = true, Value = tryResult.Result.Value };
            }
        }

        public ModbusTcpReadResult ReadInputRegister(string ip, int port, int address)
        {
            const byte CLIENT_PURPOSE = 2;

            var getClientResult = TryGetConnectedClient(ip, port, CLIENT_PURPOSE);

            if (getClientResult.Exception != null)
            {
                return new ModbusTcpReadResult { Success = false, Exception = getClientResult.Exception };
            }

            if (getClientResult.Client == null)
            {
                return new ModbusTcpReadResult { Success = false, Exception = new Exception("Client cannot be null.") };
            }

            lock (getClientResult.Client)
            {
                var tryResult = Try(() => getClientResult.Client.ReadInputRegisters(address, 3)[0], RETRY_ATTEMPTS_COUNT, RETRY_INTERVAL_MILLISECONDS);

                if (tryResult.Exception != null)
                {
                    Console.WriteLine($"ModbusTcp - Read IR {address}@{ip}:{port} failed.");
                    RemoveDeadClient(ip, port, CLIENT_PURPOSE);

                    return new ModbusTcpReadResult { Success = false, Exception = tryResult.Exception };
                }

                return new ModbusTcpReadResult { Success = true, Value = tryResult.Result.Value };
            }
        }

        public ModbusTcpWriteResult WriteHoldingRegister(string ip, int port, int address, int value)
        {
            const byte CLIENT_PURPOSE = 1;

            var getClientResult = TryGetConnectedClient(ip, port, CLIENT_PURPOSE);

            if (getClientResult.Exception != null)
            {
                return new ModbusTcpWriteResult { Success = false, Exception = getClientResult.Exception };
            }

            if (getClientResult.Client == null)
            {
                return new ModbusTcpWriteResult { Success = false, Exception = new Exception("Client cannot be null.") };
            }

            lock (getClientResult.Client)
            {
                var tryResult = Try(() => getClientResult.Client.WriteSingleRegister(address, value), RETRY_ATTEMPTS_COUNT, RETRY_INTERVAL_MILLISECONDS);

                if (tryResult != null)
                {
                    Console.WriteLine($"ModbusTcp - Write HR {address}@{ip}:{port} failed.");
                    RemoveDeadClient(ip, port, CLIENT_PURPOSE);

                    return new ModbusTcpWriteResult { Success = false, Exception = tryResult };
                }

                return new ModbusTcpWriteResult { Success = true };
            }
        }

        private (EasyModbus.ModbusClient Client, Exception Exception) TryGetConnectedClient(string ip, int port, byte p)
        {
            lock (_clientLock)
            {
                var modbusDescriptor = ModbusServerDescriptor.Create(ip, port, p);
                var client = _modbusClientCache.GetValueOrDefault(modbusDescriptor);

                try
                {
                    if (client == null)
                    {
                        client = new EasyModbus.ModbusClient(ip, port);
                        _modbusClientCache.Add(modbusDescriptor, client);
                    }

                    if (!client.Connected)
                    {
                        client.Connect();
                    }
                }
                catch (Exception exception)
                {
                    Logger.DebugWithData("ModbusTcp connection error", exception.ToString());
                    client.Disconnect();
                    _modbusClientCache.Remove(modbusDescriptor);

                    return (null, exception);
                }

                return (client, null);
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

        private Exception Try(Action action, int attempts, int millisecondsDelay)
        {
            return Try(() => { action(); return 0; }, attempts, millisecondsDelay).Exception;
        }

        private (T? Result, Exception Exception) Try<T>(Func<T> func, int attempts, int millisecondsDelay) where T : struct
        {
            var attemptNumber = 0;
            Exception lastException = null;

            while (attemptNumber < attempts)
            {
                attemptNumber++;
                Task.Delay(millisecondsDelay).Wait();

                try
                {
                    return (func(), null);
                }
                catch (Exception e)
                {
                    lastException = e;
                }
            }

            return (null, lastException);
        }

        private struct ModbusServerDescriptor
        {
            public string IpAddress { get; set; }
            public int PortNumber { get; set; }
            public byte Purpose { get; set; }
            public static ModbusServerDescriptor Create(string ip, int port, byte purpose) => new ModbusServerDescriptor { IpAddress = ip, PortNumber = port, Purpose = 0 };
        }
    }
}
