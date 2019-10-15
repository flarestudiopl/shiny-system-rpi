using Domain;
using HardwareAccess.Devices.DigitalInputs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HardwareAccess.Dummy.Devices.DigitalInputs
{
    public class ShinyPowerState : IShinyPowerState
    {
        private readonly IDictionary<string, bool> InputNameToFakeState = new Dictionary<string, bool>
        {
            ["AC OK"] = true,
            ["Low bat"] = false
        };

        public string ProtocolName => ProtocolNames.ShinyBoard;

        public ICollection<string> InputNames => InputNameToFakeState.Keys;

        public Task<ICollection<int>> GetDeviceIds()
        {
            return Task.FromResult((ICollection<int>)new int[] { 0 });
        }

        public Task<bool> GetState(int deviceId, string inputName)
        {
           return Task.FromResult(InputNameToFakeState[inputName]);
        }
    }
}
