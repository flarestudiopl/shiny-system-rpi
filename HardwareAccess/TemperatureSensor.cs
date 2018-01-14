using Commons;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareAccess
{
    public interface ITemperatureSensor
    {
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

        public async Task<TemperatureSensorData> Read(string deviceId)
        {
            if (SupportedDevicesPrefix.Any(x => deviceId.StartsWith(x)))
            {
                var rawData = await _oneWire.GetDeviceData(deviceId);
                var indexOfTemp = rawData.IndexOf("t=");

                if (indexOfTemp > 0)
                {
                    var result = new TemperatureSensorData
                    {
                        CrcOk = rawData.Contains("YES")
                    };

                    if (int.TryParse(rawData.Substring(indexOfTemp + 2, 5), out int temp))
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

            Logger.Warning("Unsupported device.");

            return new TemperatureSensorData();
        }
    }
}
