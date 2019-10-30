using Domain;
using HardwareAccess.Buses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareAccess.Devices.PowerOutputs
{
    public interface IInvertedPcfOutput : IPowerOutput
    {
    }

    public class InvertedPcfOutput : IInvertedPcfOutput
    {
        private readonly static int[] I2C_ADDRESSES = new int[] { 32, 56, 57, 58, 59, 60, 61, 62, 63 };

        private readonly static IDictionary<string, int> OUTPUT_NAME_TO_CHANNEL = new Dictionary<string, int>
        {
            ["O1"] = 1,
            ["O2"] = 2,
            ["O3"] = 3,
            ["O4"] = 4,
            ["O5"] = 5,
            ["O6"] = 6,
            ["O7"] = 7,
            ["O8"] = 8
        };

        private readonly II2c _i2c;

        private readonly object _lock = new object();
        private readonly IDictionary<int, int> _deviceToOutputState = new Dictionary<int, int>();

        public string ProtocolName => ProtocolNames.InvertedPcf;

        public ICollection<string> OutputNames => OUTPUT_NAME_TO_CHANNEL.Keys;

        public InvertedPcfOutput(II2c i2c)
        {
            _i2c = i2c;
        }

        public async Task<ICollection<int>> GetDeviceIds()
        {
            var i2cDevices = await _i2c.GetI2cDevices();

            return i2cDevices.Intersect(I2C_ADDRESSES)
                             .ToArray();
        }

        public void SetState(int deviceId, string outputName, bool state)
        {
            if (!OUTPUT_NAME_TO_CHANNEL.TryGetValue(outputName, out var channel))
            {
                throw new ArgumentException(nameof(outputName));
            }

            SetChannelState(deviceId, channel, state);
        }

        public bool GetState(int deviceId, string outputName)
        {
            if (!OUTPUT_NAME_TO_CHANNEL.TryGetValue(outputName, out var channel))
            {
                throw new ArgumentException(nameof(outputName));
            }

            return GetChannelState(deviceId, channel);
        }

        private void SetChannelState(int deviceId, int channel, bool state)
        {
            if (!(channel >= 1 && channel <= 8))
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }

            var bitToFlip = 1 << channel - 1;

            lock (_lock)
            {
                var currentState = 0;

                _deviceToOutputState.TryGetValue(deviceId, out currentState);

                if (state)
                {
                    currentState |= bitToFlip;
                }
                else
                {
                    currentState &= ~bitToFlip;
                }

                _deviceToOutputState[deviceId] = currentState;

                _i2c.WriteToDevice(deviceId, (byte)~currentState);
            }
        }

        private bool GetChannelState(int deviceId, int channel)
        {
            var bitToCheck = 1 << channel - 1;

            if (!_deviceToOutputState.ContainsKey(deviceId))
            {
                _deviceToOutputState[deviceId] = 0;
            }

            return (_deviceToOutputState[deviceId] & bitToCheck) == bitToCheck;
        }
    }
}
