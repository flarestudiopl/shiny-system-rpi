using Commons;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareAccess
{
    public interface IOneWire
    {
        IList<string> GetDevicesList();
        Task<string> GetDeviceData(string deviceId);
    }

    public class OneWire : IOneWire
    {
        private const string OneWirePath = "/sys/bus/w1/devices";
        private const string OneWireDevicePathTemplate = "/sys/bus/w1/devices/{0}/w1_slave";

        public IList<string> GetDevicesList()
        {
            if (Directory.Exists(OneWirePath))
            {
                return Directory.EnumerateDirectories(OneWirePath).Select(x => Path.GetFileName(x)).ToList();
            }
            else
            {
                Logger.Warning("Cannot access 1-wire directory.");
            }

            return new List<string> { "dummy-device" };
        }

        public async Task<string> GetDeviceData(string deviceId)
        {
            var devicePath = OneWireDevicePathTemplate.FormatWith(deviceId);

            if (File.Exists(devicePath))
            {
                return await File.ReadAllTextAsync(devicePath);
            }
            else
            {
                Logger.Warning("Cannot access 1-wire device directory.");
            }

            return string.Empty;
        }
    }
}
