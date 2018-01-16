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
        private const int OPEN_READ_WRITE_MODE = 2;
        private const int I2C_SLAVE_REQUEST = 0x0703;

        private readonly IProcessRunner _processRunner;
        private readonly int _i2cBusHandle;

        public I2c(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
            _i2cBusHandle = LibcWrapper.Open("/dev/i2c-1", OPEN_READ_WRITE_MODE);
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
            if (LibcWrapper.Ioctl(_i2cBusHandle, I2C_SLAVE_REQUEST, i2cDevice) >= 0)
            {
                LibcWrapper.Write(_i2cBusHandle, new[] { value }, 1);
            }
            else
            {
                Logger.Warning("IOCTL < 0");
            }
        }
    }
}
