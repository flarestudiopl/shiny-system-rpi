using System.Collections.Generic;
using System.Threading.Tasks;

namespace HardwareAccess.Devices.DigitalInputs
{
    public interface IDigitalInput
    {
        string ProtocolName { get; }

        ICollection<string> InputNames { get; }

        Task<ICollection<int>> GetDeviceIds();

        Task<bool> GetState(int deviceId, string inputName);
    }
}
