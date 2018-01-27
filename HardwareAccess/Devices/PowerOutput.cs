﻿using HardwareAccess.Buses;
using System;
using System.Collections.Generic;
using System.Text;

namespace HardwareAccess.Devices
{
    public interface IPowerOutput
    {
        void SetState(int deviceId, int channel, bool state);
    }

    public class PowerOutput : IPowerOutput
    {
        private readonly II2c _i2c;

        private readonly object _lock = new object();
        private readonly IDictionary<int, int> _deviceToOutputState = new Dictionary<int, int>();

        public PowerOutput(II2c i2c)
        {
            _i2c = i2c;
        }

        public void SetState(int deviceId, int channel, bool state)
        {
            if (!(channel >= 1 && channel <= 8))
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }

            var bitToFlip = (int)Math.Pow(2, channel - 1);

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

                _i2c.WriteToDevice(deviceId, (byte)currentState);
            }
        }
    }
}