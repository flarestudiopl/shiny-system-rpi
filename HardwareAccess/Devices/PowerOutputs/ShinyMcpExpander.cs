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
        private readonly static HashSet<int> I2C_ADDRESSES = new HashSet<int>(new int[] { 32, 33, 34, 35 });

        private readonly static IDictionary<string, Channel> OUTPUT_NAME_TO_CHANNEL = new Dictionary<string, Channel>
        {
            ["O1"] = Channel.Create(Bank.B, 7),
            ["O2"] = Channel.Create(Bank.B, 6),
            ["O3"] = Channel.Create(Bank.B, 5),
            ["O4"] = Channel.Create(Bank.B, 4),
            ["O5"] = Channel.Create(Bank.B, 3),
            ["O6"] = Channel.Create(Bank.B, 2),
            ["O7"] = Channel.Create(Bank.B, 1),
            ["O8"] = Channel.Create(Bank.B, 0),
            ["O9"] = Channel.Create(Bank.A, 3),
            ["O10"] = Channel.Create(Bank.A, 2),
            ["N1"] = Channel.Create(Bank.A, 1),
            ["N2"] = Channel.Create(Bank.A, 0),
            ["AUX1"] = Channel.Create(Bank.A, 4),
            ["AUX2"] = Channel.Create(Bank.A, 5),
            ["AUX3"] = Channel.Create(Bank.A, 6),
            ["AUX4"] = Channel.Create(Bank.A, 7)
        };

        private readonly II2c _i2c;

        private readonly object _lock = new object();
        private readonly IDictionary<int, OutputState> _deviceIdToChipState = new Dictionary<int, OutputState>();

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

        public void SetState(int deviceId, string outputName, bool newState)
        {
            var channel = GetChannel(outputName);

            lock (_lock)
            {
                var chipState = GetChipState(deviceId);

                if (!chipState.InitializedAsOutput)
                {
                    _i2c.WriteToDevice(deviceId, 0x00, 0x00);
                    _i2c.WriteToDevice(deviceId, 0x01, 0x00);
                }

                var bitToFlip = 1 << channel.Pin;

                var currentState = chipState.BankToValue[channel.Bank];

                if (newState)
                {
                    currentState |= bitToFlip;
                }
                else
                {
                    currentState &= ~bitToFlip;
                }

                chipState.BankToValue[channel.Bank] = currentState;

                _i2c.WriteToDevice(deviceId, (byte)channel.Bank, (byte)currentState);
            }
        }

        public bool GetState(int deviceId, string outputName)
        {
            var channel = GetChannel(outputName);
            var chipState = GetChipState(deviceId);

            var bitToCheck = 1 << channel.Pin;

            return (chipState.BankToValue[channel.Bank] & bitToCheck) == bitToCheck;
        }

        private Channel GetChannel(string outputName)
        {
            if (!OUTPUT_NAME_TO_CHANNEL.TryGetValue(outputName, out var channel))
            {
                throw new ArgumentException(nameof(outputName));
            }

            return channel;
        }

        private OutputState GetChipState(int deviceId)
        {
            if (!I2C_ADDRESSES.Contains(deviceId))
            {
                throw new ArgumentException(nameof(deviceId));
            }

            if (!_deviceIdToChipState.TryGetValue(deviceId, out var outputState))
            {
                outputState = new OutputState();
                _deviceIdToChipState[deviceId] = outputState;
            }

            return outputState;
        }

        private struct Channel
        {
            public Bank Bank { get; set; }
            public int Pin { get; set; }

            public static Channel Create(Bank bank, int pin) => new Channel { Bank = bank, Pin = pin };
        }

        private class OutputState
        {
            public bool InitializedAsOutput { get; set; }
            public IDictionary<Bank, int> BankToValue { get; } = new Dictionary<Bank, int> { [Bank.A] = 0, [Bank.B] = 0 };
        }

        private enum Bank : byte
        {
            A = 0x14,
            B = 0x15
        }
    }
}
