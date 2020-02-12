using Commons;
using Domain;
using HardwareAccess.Buses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HardwareAccess.Devices.PowerOutputs
{
    public interface IInvertedPcfOutput : IPowerOutput
    {
    }

    public class InvertedPcfOutput : IInvertedPcfOutput
    {
        private readonly static int[] I2C_ADDRESSES = new int[] { 32, 56, 57, 58, 59, 60, 61, 62, 63 };

        public struct OutputDescriptor
        {
            public int DeviceId { get; set; }
            public string OutputName { get; set; }
        }

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

        public object ConfigurationOptions => new { OutputNames = OUTPUT_NAME_TO_CHANNEL.Keys, DeviceIds = GetDeviceIds() };

        public Type OutputDescriptorType => typeof(OutputDescriptor);

        public InvertedPcfOutput(II2c i2c)
        {
            _i2c = i2c;
        }

        public void SetState(object outputDescriptor, bool state)
        {
            var output = CastOutputDescriptorOrThrow(outputDescriptor);

            Logger.DebugWithData("New output state: ", new { output.DeviceId, output.OutputName, state });

            if (!OUTPUT_NAME_TO_CHANNEL.TryGetValue(output.OutputName, out var channel))
            {
                throw new ArgumentException(nameof(output.OutputName));
            }

            SetChannelState(output.DeviceId, channel, state);
        }

        public bool GetState(object outputDescriptor)
        {
            var output = CastOutputDescriptorOrThrow(outputDescriptor);

            if (!OUTPUT_NAME_TO_CHANNEL.TryGetValue(output.OutputName, out var channel))
            {
                throw new ArgumentException(nameof(output.OutputName));
            }

            return GetChannelState(output.DeviceId, channel);
        }

        private ICollection<int> GetDeviceIds()
        {
            var i2cDevices = _i2c.GetI2cDevices();

            if (i2cDevices.Wait(500))
            {
                return i2cDevices.Result
                                 .Intersect(I2C_ADDRESSES)
                                 .ToArray();
            }

            return new int[0];
        }

        private OutputDescriptor CastOutputDescriptorOrThrow(object outputDescriptor)
        {
            if (outputDescriptor is OutputDescriptor)
            {
                return (OutputDescriptor)outputDescriptor;
            }

            throw new ArgumentException("Output descriptor -- protocol mismatch.");
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
