using Commons;
using Domain;
using HardwareAccess.Buses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareAccess.Devices.TemperatureInputs
{
    public interface IDs1820 : ITemperatureInput { }
    public class Ds1820 : IDs1820
    {
        public struct InputDescriptor
        {
            public string DeviceId { get; set; }
        }

        private static readonly string[] SupportedDevicesPrefix = { "10", "28" };

        private readonly IOneWire _oneWire;

        public string ProtocolName => ProtocolNames.Ds1820;

        public object ConfigurationOptions => new { AvailableSensors = GetAvailableSensors() };

        public Type InputDescriptorType => typeof(InputDescriptor);

        public Ds1820(IOneWire oneWire)
        {
            _oneWire = oneWire;
        }

        public async Task<TemperatureSensorData> GetValue(object inputDescriptor)
        {
            var input = DescriptorHelper.CastHardwareDescriptorOrThrow<InputDescriptor>(inputDescriptor);

            if (SupportedDevicesPrefix.Any(input.DeviceId.StartsWith))
            {
                var rawData = await _oneWire.GetDeviceData(input.DeviceId);
                var indexOfTemp = rawData.IndexOf("t=", StringComparison.Ordinal) + 2;

                if (indexOfTemp > 2)
                {
                    var result = new TemperatureSensorData
                    {
                        Success = rawData.Contains("YES") && !rawData.Contains("00 00 00 00 00 00 00 00 00")
                    };

                    if (int.TryParse(rawData.Substring(indexOfTemp, rawData.Length - (indexOfTemp + 1)), out int temp))
                    {
                        result.Value = temp / 1000d;
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

        private ICollection<string> GetAvailableSensors()
        {
            return _oneWire.GetDevicesList()
                           .Where(x => x.Length > 2 && SupportedDevicesPrefix.Contains(x.Substring(0, 2)))
                           .ToList();
        }
    }
}
