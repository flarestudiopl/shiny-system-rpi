using System.Collections.Generic;
using System.Threading.Tasks;

namespace HardwareAccess.Devices.PowerOutputs
{
    public interface IPowerOutput
    {
        string ProtocolName { get; }

        ICollection<string> OutputNames { get; }

        Task<ICollection<int>> GetDeviceIds();

        void SetState(int deviceId, string outputName, bool state);

        bool GetState(int deviceId, string outputName);
    }
}
