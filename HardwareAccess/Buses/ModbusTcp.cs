using NModbus;
using System;
using System.Net.Sockets;
using System.Threading;

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
        private AutoResetEvent _autoResetEvent = new AutoResetEvent(true);

        public ModbusTcpReadResult ReadHoldingRegister(string ip, int port, int address)
        {
            _autoResetEvent.WaitOne();

            try
            {
                using (TcpClient client = new TcpClient(ip, port) { ReceiveTimeout = 1000, SendTimeout = 1000 })
                {
                    var factory = new ModbusFactory();
                    using (var master = factory.CreateMaster(client))
                    {
                        var value = master.ReadHoldingRegisters(1, (ushort)address, 1)[0];
                        return new ModbusTcpReadResult { Value = value, Success = true };
                    }
                }
            }
            catch (Exception exception)
            {
                return new ModbusTcpReadResult { Success = false, Exception = exception };
            }
            finally
            {
                _autoResetEvent.Set();
            }
        }

        public ModbusTcpReadResult ReadInputRegister(string ip, int port, int address)
        {
            _autoResetEvent.WaitOne();

            try
            {
                using (TcpClient client = new TcpClient(ip, port) { ReceiveTimeout = 1000, SendTimeout = 1000 })
                {
                    var factory = new ModbusFactory();
                    using (var master = factory.CreateMaster(client))
                    {
                        var value = master.ReadInputRegisters(1, (ushort)address, 1)[0];
                        return new ModbusTcpReadResult { Value = value, Success = true };
                    }
                }
            }
            catch (Exception exception)
            {
                return new ModbusTcpReadResult { Success = false, Exception = exception };
            }
            finally
            {
                _autoResetEvent.Set();
            }
        }

        public ModbusTcpWriteResult WriteHoldingRegister(string ip, int port, int address, int value)
        {
            _autoResetEvent.WaitOne();

            try
            {
                using (TcpClient client = new TcpClient(ip, port) { ReceiveTimeout = 1000, SendTimeout = 1000 })
                {
                    var factory = new ModbusFactory();
                    using (var master = factory.CreateMaster(client))
                    {
                        master.WriteSingleRegister(1, (ushort)address, (ushort)value);
                        return new ModbusTcpWriteResult { Success = true };
                    }
                }
            }
            catch (Exception exception)
            {
                return new ModbusTcpWriteResult { Success = false, Exception = exception };
            }
            finally
            {
                _autoResetEvent.Set();
            }
        }
    }
}
