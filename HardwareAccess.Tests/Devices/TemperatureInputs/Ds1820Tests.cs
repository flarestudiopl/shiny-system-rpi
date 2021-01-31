using HardwareAccess.Buses;
using HardwareAccess.Devices.TemperatureInputs;
using NSubstitute;
using Xunit;

namespace HardwareAccess.Tests.Devices.TemperatureInputs
{
    public class Ds1820Tests
    {
        private static TemperatureSensorData Act(string rawData)
        {
            var deviceId = "28-00000595d87e";

            var oneWire = Substitute.For<IOneWire>();
            oneWire.GetDeviceData(null).ReturnsForAnyArgs(rawData);

            var temperatureSensor = new Ds1820(oneWire);
            var result = temperatureSensor.GetValue(new Ds1820.InputDescriptor { DeviceId = deviceId });

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
            Assert.False(result.Success);
        }

        [Fact]
        public void detects_empty_response_as_crc_error()
        {
            // Arrange
            var rawData = "00 00 00 00 00 00 00 00 00 : crc=00 YES\n00 00 00 00 00 00 00 00 00 t=0\n";

            // Act
            var result = Act(rawData);

            // Assert
            Assert.False(result.Success);
        }
    }
}
