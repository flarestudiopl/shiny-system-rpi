using Domain;
using HardwareAccess.Buses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareAccess.Devices.PowerOutputs
{
    public interface IShinyMcpExpander : IPowerOutput
    {
    }

    public class ShinyMcpExpander : IShinyMcpExpander
    {
        private readonly static int[] I2C_ADDRESSES = new int[] { 20, 21, 22, 23 };

        private readonly static IDictionary<string, int> OUTPUT_NAME_TO_CHANNEL = new Dictionary<string, int>
        {
            ["O1"] = 16,
            ["O2"] = 15,
            ["O3"] = 14,
            ["O4"] = 13,
            ["O5"] = 12,
            ["O6"] = 11,
            ["O7"] = 10,
            ["O8"] = 9,
            ["O9"] = 4,
            ["O10"] = 3,
            ["N1"] = 2,
            ["N2"] = 1,
        };

        private readonly II2c _i2c;

        public string ProtocolName => ProtocolNames.ShinyBoard;

        public ICollection<string> OutputNames => OUTPUT_NAME_TO_CHANNEL.Keys;

        public ShinyMcpExpander(II2c i2c)
        {
            _i2c = i2c;
        }

        public async Task<ICollection<int>> GetDeviceIds()
        {
            var i2cDevices = await _i2c.GetI2cDevices();

            return i2cDevices.Intersect(I2C_ADDRESSES)
                             .ToArray();
        }

        public bool GetState(int deviceId, string outputName)
        {
            throw new NotImplementedException();
        }

        public void SetState(int deviceId, string outputName, bool state)
        {
            throw new NotImplementedException();
        }
    }
}
