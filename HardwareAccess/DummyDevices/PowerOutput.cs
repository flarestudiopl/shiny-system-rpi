using Commons;
using HardwareAccess.Devices;
using System.Collections.Generic;

namespace HardwareAccess.DummyDevices
{
    public class PowerOutput : IPowerOutput
    {
        private readonly object _lock = new object();
        private readonly IDictionary<int, int> _deviceToOutputState = new Dictionary<int, int>();

        public void SetState(int deviceId, int channel, bool state)
        {
            var bitToFlip = 1 << (channel - 1);

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

                Logger.Trace("Device {0} has now state = {1}.", new object[] { deviceId, currentState });
            }
        }

        public bool GetState(int deviceId, int channel)
        {
            var bitToCheck = 1 << (channel - 1);

            return (_deviceToOutputState[deviceId] & bitToCheck) == bitToCheck;
        }

    }
}
