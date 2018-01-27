using HardwareAccess.Buses;
using HardwareAccess.Devices;
using NSubstitute;
using Xunit;

namespace HardwareAccess.Tests.Devices
{
    public class TemperatureSensorTests
    {
        private static TemperatureSensorData Act(string rawData)
        {
            var deviceId = "28-00000595d87e";

            var oneWire = Substitute.For<IOneWire>();
            oneWire.GetDeviceData(null).ReturnsForAnyArgs(rawData);

            var temperatureSensor = new TemperatureSensor(oneWire);
            var result = temperatureSensor.Read(deviceId);

            result.Wait();
            return result.Result;
        }

        [Fact]
        public void temperature_parsed()
        {
            // Arrange
            var rawData = "56 01 4b 46 7f ff 0a 10 d1 : crc=d1 YES\n56 01 4b 46 7f ff 0a 10 d1 t=-62\n";

            // Act
            var result = Act(rawData);

            // Assert
            Assert.Equal(-0.062f, result.Value, 3);
        }

        [Fact]
        public void detects_crc_error()
        {
            // Arrange
            var rawData = "56 01 4b 46 7f ff 0a 10 d1 : crc=e1 NO\n56 01 4b 46 7f ff 0a 10 d1 t=21375\n";

            // Act
            var result = Act(rawData);

            // Assert
            Assert.Equal(21.375f, result.Value);
            Assert.False(result.CrcOk);
        }
    }
}
