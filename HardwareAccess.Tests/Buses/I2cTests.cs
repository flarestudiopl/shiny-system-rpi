using HardwareAccess.Buses;
using HardwareAccess.PlatformIntegration;
using NSubstitute;
using Xunit;

namespace HardwareAccess.Tests.Buses
{
    public class I2cTests
    {
        [Fact]
        public void devices_list_parsed()
        {
            // Arrange
            var i2cdetectResult = "     0  1  2  3  4  5  6  7  8  9  a  b  c  d  e  f\n" +
                                  "00:          -- -- -- -- -- -- -- -- -- -- -- -- --\n" +
                                  "10: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --\n" +
                                  "20: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --\n" +
                                  "30: -- -- -- -- -- -- -- -- -- -- -- -- 3c -- -- --\n" +
                                  "40: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --\n" +
                                  "50: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --\n" +
                                  "60: -- -- -- -- -- -- -- -- UU -- -- -- -- -- -- --\n" +
                                  "70: -- -- -- -- -- -- -- --\n";

            var processRunner = Substitute.For<IProcessRunner>();
            processRunner.Run("i2cdetect", "-y 1").Returns(i2cdetectResult);

            // Act
            var i2c = new I2c(processRunner, null);
            var i2cDevices = i2c.GetI2cDevices();
            i2cDevices.Wait();

            // Assert
            Assert.Collection(i2cDevices.Result, id => Assert.Equal(0x3c, id));
        }
    }
}
