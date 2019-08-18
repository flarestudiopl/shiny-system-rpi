using Commons;
using Commons.Exceptions;
using HardwareAccess.PlatformIntegration;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareAccess.Buses
{
    public interface II2c
    {
        Task<int[]> GetI2cDevices();
        void WriteToDevice(int i2cDevice, byte value);
        void WriteToDevice(int i2cDevice, byte register, byte value);
    }

    public class I2c : II2c
    {
        private const int I2C_SLAVE_REQUEST = 0x0703;

        private readonly IProcessRunner _processRunner;
        private readonly ILibcWrapper _libcWrapper;
        private readonly Lazy<int> _i2cBusHandle;

        public I2c(IProcessRunner processRunner, ILibcWrapper libcWrapper)
        {
            _processRunner = processRunner;
            _libcWrapper = libcWrapper;
            _i2cBusHandle = new Lazy<int>(() => _libcWrapper.Open("/dev/i2c-1", LibcOpenMode.Write));
        }

        public async Task<int[]> GetI2cDevices()
        {
            var detectResult = await _processRunner.Run("i2cdetect", "-y 1");

            try
            {
                var addresses = detectResult.Split('\n')
                                            .Where(x => x.Contains(':'))
                                            .Select(x => x.Split(':')[1])
                                            .SelectMany(x => x.Split(' '))
                                            .Where(x => !string.IsNullOrWhiteSpace(x) && x != "--" && x != "UU")
                                            .Select(x => int.Parse(x, NumberStyles.AllowHexSpecifier))
                                            .ToArray();

                return addresses;
            }
            catch (Exception exception)
            {
                throw new ParseException(exception, detectResult);
            }
        }

        public void WriteToDevice(int i2cDevice, byte value)
        {
            I2cSlaveWrite(i2cDevice, new[] { value });
        }

        public void WriteToDevice(int i2cDevice, byte register, byte value)
        {
            I2cSlaveWrite(i2cDevice, new[] { register, value });
        }

        private void I2cSlaveWrite(int i2cDevice, byte[] data)
        {
            if (_libcWrapper.SendControl(_i2cBusHandle.Value, I2C_SLAVE_REQUEST, i2cDevice) >= 0)
            {
                _libcWrapper.Write(_i2cBusHandle.Value, data);
            }
            else
            {
                Logger.Warning("IOCTL < 0");
            }
        }
    }
}
