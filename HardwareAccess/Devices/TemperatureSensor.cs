using Commons;
using HardwareAccess.Buses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareAccess.Devices
{
    public interface ITemperatureSensor
    {
        ICollection<string> GetAvailableSensors();
        Task<TemperatureSensorData> Read(string deviceId);
    }

    public struct TemperatureSensorData
    {
        public float Value { get; set; }
        public bool CrcOk { get; set; }
    }

    public class TemperatureSensor : ITemperatureSensor
    {
        private static string[] SupportedDevicesPrefix = new[] { "10", "28" };

        private readonly IOneWire _oneWire;

        public TemperatureSensor(IOneWire oneWire)
        {
            _oneWire = oneWire;
        }

        public ICollection<string> GetAvailableSensors()
        {
            return _oneWire.GetDevicesList()
                           .Where(x => x.Length > 2 && SupportedDevicesPrefix.Contains(x.Substring(0, 2)))
                           .ToList();
        }

        public async Task<TemperatureSensorData> Read(string deviceId)
        {
            if (SupportedDevicesPrefix.Any(x => deviceId.StartsWith(x)))
            {
                var rawData = await _oneWire.GetDeviceData(deviceId);
                var indexOfTemp = rawData.IndexOf("t=") + 2;

                if (indexOfTemp > 2)
                {
                    var result = new TemperatureSensorData
                    {
                        CrcOk = rawData.Contains("YES") && !rawData.Contains("00 00 00 00 00 00 00 00 00")
                    };

                    if (int.TryParse(rawData.Substring(indexOfTemp, rawData.Length - (indexOfTemp + 1)), out int temp))
                    {
                        result.Value = temp / 1000f;
                    }

                    return result;
                }
                else
                {
                    Logger.Warning("No data.");
                }
            }
            else
            {
                Logger.Warning("Unsupported device.");
            }

            return new TemperatureSensorData();
        }
    }
}
