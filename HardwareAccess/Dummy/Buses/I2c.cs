using System.Collections.Generic;
using System.Threading.Tasks;
using HardwareAccess.Buses;

namespace HardwareAccess.Dummy.Buses
{
    public class I2c : II2c
    {
        public Task<IList<int>> GetI2cDevices()
        {
            return Task.FromResult<IList<int>>(new List<int> { 56, 57 });
        }

        public void WriteToDevice(int i2cDevice, byte value)
        {
        }
    }
}
