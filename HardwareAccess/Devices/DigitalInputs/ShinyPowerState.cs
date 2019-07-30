using Domain;
using HardwareAccess.PlatformIntegration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HardwareAccess.Devices.DigitalInputs
{
    public interface IShinyPowerState : IDigitalInput
    {
    }

    public class ShinyPowerState : IShinyPowerState
    {
        private readonly IDictionary<string, byte> InputNameToGpioNumber = new Dictionary<string, byte>
        {
            ["AC OK"] = 20,
            ["Low bat"] = 21
        };

        private readonly IDictionary<string, int> InputNameToValueHandle = new Dictionary<string, int>();

        private readonly ILibcWrapper _libcWrapper;

        public string ProtocolName => ProtocolNames.ShinyBoard;

        public ICollection<string> InputNames => InputNameToGpioNumber.Keys;

        public ShinyPowerState(ILibcWrapper libcWrapper)
        {
            _libcWrapper = libcWrapper;

            Initialize();
        }

        public Task<ICollection<int>> GetDeviceIds()
        {
            return Task.FromResult((ICollection<int>)new int[] { 0 });
        }

        public Task<bool> GetState(int deviceId, string inputName)
        {
            int handle;
            if (!InputNameToValueHandle.TryGetValue(inputName, out handle))
            {
                throw new ArgumentException(nameof(inputName));
            }

            var result = _libcWrapper.Read(handle, 1);
            var parsedResult = Encoding.UTF8.GetString(result);

            return Task.FromResult(parsedResult == "0");
        }

        private void Initialize()
        {
            var exportHandle = _libcWrapper.Open("/sys/class/gpio/export", LibcOpenMode.Write);

            foreach (var input in InputNameToGpioNumber)
            {
                var gpioNumber = input.Value;

                _libcWrapper.Write(exportHandle, new byte[] { gpioNumber });

                var directionHandle = _libcWrapper.Open(GetGpioDirectionPath(gpioNumber), LibcOpenMode.Write);
                _libcWrapper.Write(gpioNumber, Encoding.UTF8.GetBytes("1"));
                _libcWrapper.Close(directionHandle);

                var valueHandle = _libcWrapper.Open(GetGpioValuePath(gpioNumber), LibcOpenMode.Read);
                InputNameToValueHandle.Add(input.Key, valueHandle);
            }

            _libcWrapper.Close(exportHandle);
        }

        private string GetGpioPath(byte gpioNumber) => $"/sys/class/gpio/gpio{gpioNumber}";
        private string GetGpioValuePath(byte gpioNumber) => $"{GetGpioPath(gpioNumber)}/value";
        private string GetGpioDirectionPath(byte gpioNumber) => $"{GetGpioPath(gpioNumber)}/direction";
    }
}
