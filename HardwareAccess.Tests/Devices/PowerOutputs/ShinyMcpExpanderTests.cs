using HardwareAccess.Buses;
using HardwareAccess.Devices.PowerOutputs;
using NSubstitute;
using Xunit;

namespace HardwareAccess.Tests.Devices.PowerOutputs
{
    public class ShinyMcpExpanderTests
    {
        private object BuildOutputDescriptor(int deviceId, string outputName) => new ShinyMcpExpander.OutputDescriptor { DeviceId = deviceId, OutputName = outputName };

        [Fact]
        public void can_enable_disable_specific_bit()
        {
            // Arrange
            var deviceId = 32;
            var deviceState = 0;
            var i2c = Substitute.For<II2c>();

            i2c.When(x => x.WriteToDevice(Arg.Any<int>(), Arg.Any<byte>(), Arg.Any<byte>()))
               .Do(x => deviceState = x.ArgAt<byte>(2));

            var powerOutput = new ShinyMcpExpander(i2c);

            // Act
            powerOutput.TrySetState(BuildOutputDescriptor(deviceId, "O1"), true);  // 1000 0000
            powerOutput.TrySetState(BuildOutputDescriptor(deviceId, "O3"), true);  // 1010 0000
            powerOutput.TrySetState(BuildOutputDescriptor(deviceId, "O6"), true);  // 1010 0100
            powerOutput.TrySetState(BuildOutputDescriptor(deviceId, "O3"), false); // 1000 0100 => 132

            // Assert
            Assert.Equal(132, deviceState);
            i2c.Received().WriteToDevice(deviceId, 0x15, 132);
        }

        [Fact]
        public void can_get_state()
        {
            // Arrange
            var powerOutput = new ShinyMcpExpander(null);

            // Act
            var state = powerOutput.TryGetState(BuildOutputDescriptor(32, "O1"));

            // Assert
            Assert.Equal(false, state);
        }

        [Fact]
        public void can_set_and_read_state()
        {
            // Arrange
            var deviceId = 32;
            var i2c = Substitute.For<II2c>();
            var powerOutput = new ShinyMcpExpander(i2c);

            // Act
            powerOutput.TrySetState(BuildOutputDescriptor(deviceId, "O3"), true);  // 0000 0100

            // Assert
            Assert.Equal(false, powerOutput.TryGetState(BuildOutputDescriptor(deviceId, "O1")));
            Assert.Equal(false, powerOutput.TryGetState(BuildOutputDescriptor(deviceId, "O2")));
            Assert.Equal(true, powerOutput.TryGetState(BuildOutputDescriptor(deviceId, "O3")));
            Assert.Equal(false, powerOutput.TryGetState(BuildOutputDescriptor(deviceId, "O4")));
        }

        [Fact]
        public void initializes_output_before_setting_state()
        {
            // Arrange
            var deviceId = 32;
            var i2c = Substitute.For<II2c>();
            var powerOutput = new ShinyMcpExpander(i2c);

            // Act
            powerOutput.TrySetState(BuildOutputDescriptor(deviceId, "N1"), true);

            // Assert
            i2c.Received().WriteToDevice(deviceId, 0x00, 0x00);
            i2c.Received().WriteToDevice(deviceId, 0x01, 0x00);
            i2c.Received().WriteToDevice(deviceId, 0x14, 0x02);
        }
    }
}
