using Commons;
using Commons.Extensions;
using NModbus;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
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
        const int RETRY_ATTEMPTS_COUNT = 3;
        const int RETRY_INTERVAL_MILLISECONDS = 100;

        private object _clientLock = new object();
        private readonly IDictionary<ModbusServerDescriptor, IModbusMaster> _modbusClientCache = new Dictionary<ModbusServerDescriptor, IModbusMaster>();

        private AutoResetEvent _autoResetEvent = new AutoResetEvent(true);

        public ModbusTcpReadResult ReadHoldingRegister(string ip, int port, int address)
        {
            var getClientResult = TryGetConnectedClient(ip, port);

            if (getClientResult.Exception != null)
            {
                return new ModbusTcpReadResult { Success = false, Exception = getClientResult.Exception };
            }

            if (getClientResult.Client == null)
            {
                return new ModbusTcpReadResult { Success = false, Exception = new Exception("Client cannot be null.") };
            }

            var tryResult = Try(() => getClientResult.Client.ReadHoldingRegisters(1, (ushort)address, 1)[0], RETRY_ATTEMPTS_COUNT, RETRY_INTERVAL_MILLISECONDS);

            if (tryResult.Exception != null)
            {
                Console.WriteLine($"ModbusTcp - Read HR {address}@{ip}:{port} failed.");
                RemoveDeadClient(ip, port);

                return new ModbusTcpReadResult { Success = false, Exception = tryResult.Exception };
            }

            return new ModbusTcpReadResult { Success = true, Value = tryResult.Result.Value };

        }

        public ModbusTcpReadResult ReadInputRegister(string ip, int port, int address)
        {
            var getClientResult = TryGetConnectedClient(ip, port);

            if (getClientResult.Exception != null)
            {
                return new ModbusTcpReadResult { Success = false, Exception = getClientResult.Exception };
            }

            if (getClientResult.Client == null)
            {
                return new ModbusTcpReadResult { Success = false, Exception = new Exception("Client cannot be null.") };
            }

            var tryResult = Try(() => getClientResult.Client.ReadInputRegisters(1, (ushort)address, 1)[0], RETRY_ATTEMPTS_COUNT, RETRY_INTERVAL_MILLISECONDS);

            if (tryResult.Exception != null)
            {
                Console.WriteLine($"ModbusTcp - Read IR {address}@{ip}:{port} failed.");
                RemoveDeadClient(ip, port);

                return new ModbusTcpReadResult { Success = false, Exception = tryResult.Exception };
            }

            return new ModbusTcpReadResult { Success = true, Value = tryResult.Result.Value };

        }

        public ModbusTcpWriteResult WriteHoldingRegister(string ip, int port, int address, int value)
        {
            var getClientResult = TryGetConnectedClient(ip, port);

            if (getClientResult.Exception != null)
            {
                return new ModbusTcpWriteResult { Success = false, Exception = getClientResult.Exception };
            }

            if (getClientResult.Client == null)
            {
                return new ModbusTcpWriteResult { Success = false, Exception = new Exception("Client cannot be null.") };
            }

            var tryResult = Try(() => getClientResult.Client.WriteSingleRegister(1, (ushort)address, (ushort)value), RETRY_ATTEMPTS_COUNT, RETRY_INTERVAL_MILLISECONDS);

            if (tryResult != null)
            {
                Console.WriteLine($"ModbusTcp - Write HR {address}@{ip}:{port} failed.");
                RemoveDeadClient(ip, port);

                return new ModbusTcpWriteResult { Success = false, Exception = tryResult };
            }

            return new ModbusTcpWriteResult { Success = true };

        }

        private (IModbusMaster Client, Exception Exception) TryGetConnectedClient(string ip, int port)
        {
            lock (_clientLock)
            {
                var modbusDescriptor = ModbusServerDescriptor.Create(ip, port);
                var client = _modbusClientCache.GetValueOrDefault(modbusDescriptor);
                TcpClient tcpClient = null;

                try
                {
                    if (client == null)
                    {
                        Logger.DebugWithData("Trying to create new ModbusMaster", new { ip, port });

                        tcpClient = new TcpClient(ip, port)
                        {
                            ReceiveTimeout = 500,
                            SendTimeout = 500
                        };

                        var modbusFactory = new ModbusFactory();

                        client = modbusFactory.CreateMaster(tcpClient);
                        _modbusClientCache.Add(modbusDescriptor, client);
                    }
                }
                catch (Exception exception)
                {
                    Logger.DebugWithData("ModbusTcp connection error", exception.ToString());
                    tcpClient?.Dispose();
                    client?.Dispose();
                    _modbusClientCache.Remove(modbusDescriptor);

                    return (null, exception);
                }

                return (client, null);
            }
        }

        private void RemoveDeadClient(string ip, int port)
        {
            lock (_clientLock)
            {
                var modbusDescriptor = ModbusServerDescriptor.Create(ip, port);

                if (_modbusClientCache.TryGetValue(modbusDescriptor, out var client))
                {
                    _modbusClientCache[modbusDescriptor].Dispose();
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

                try
                {
                    _autoResetEvent.WaitOne();
                    var result = func();
                    _autoResetEvent.Set();

                    return (result, null);
                }
                catch (Exception e)
                {
                    lastException = e;
                }
                finally
                {
                    Task.Delay(millisecondsDelay).Wait();
                }
            }

            return (null, lastException);
        }

        private struct ModbusServerDescriptor
        {
            public string IpAddress { get; set; }
            public int PortNumber { get; set; }
            public static ModbusServerDescriptor Create(string ip, int port) => new ModbusServerDescriptor { IpAddress = ip, PortNumber = port };
        }
    }
}
