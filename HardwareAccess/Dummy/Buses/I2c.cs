using System.Threading.Tasks;
using HardwareAccess.Buses;

namespace HardwareAccess.Dummy.Buses
{
    public class I2c : II2c
    {
        public Task<int[]> GetI2cDevices()
        {
            return Task.FromResult(new int[] { 32, 56, 57 });
        }

        public void WriteToDevice(int i2cDevice, byte value)
        {
        }

        public void WriteToDevice(int i2cDevice, byte register, byte value)
        {
        }
    }
}
