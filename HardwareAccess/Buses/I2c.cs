using Commons;
using HardwareAccess.Buses.PlatformIntegration;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareAccess.Buses
{
    public interface II2c
    {
        Task<IList<int>> GetI2cDevices();
        void WriteToDevice(int i2cDevice, byte value);
    }

    public class I2c : II2c
    {
        private const int I2C_SLAVE_REQUEST = 0x0703;

        private readonly IProcessRunner _processRunner;
        private readonly ILibcWrapper _libcWrapper;
        private readonly int _i2cBusHandle;

        public I2c(IProcessRunner processRunner, ILibcWrapper libcWrapper)
        {
            _processRunner = processRunner;
            _libcWrapper = libcWrapper;
            _i2cBusHandle = _libcWrapper.OpenReadWrite("/dev/i2c-1");
        }

        public async Task<IList<int>> GetI2cDevices()
        {
            var detectResult = await _processRunner.Run("i2cdetect", "-y 1");

            var addresses = detectResult.Split('\n')
                                        .Where(x => x.Contains(':'))
                                        .Select(x => x.Split(':')[1])
                                        .SelectMany(x => x.Split(' '))
                                        .Where(x => !string.IsNullOrWhiteSpace(x) && x != "--" && x != "UU")
                                        .Select(x => int.Parse(x, NumberStyles.AllowHexSpecifier))
                                        .ToList();

            return addresses;
        }

        public void WriteToDevice(int i2cDevice, byte value)
        {
            if (_libcWrapper.SendControl(_i2cBusHandle, I2C_SLAVE_REQUEST, i2cDevice) >= 0)
            {
                _libcWrapper.Write(_i2cBusHandle, new[] { value });
            }
            else
            {
                Logger.Warning("IOCTL < 0");
            }
        }
    }
}
