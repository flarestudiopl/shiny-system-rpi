using HardwareAccess.Buses;
using HardwareAccess.Devices.PowerOutputs;
using NSubstitute;
using Xunit;

namespace HardwareAccess.Tests.Devices.PowerOutputs
{
    public class InvertedPcfOutputTests
    {
        private object BuildOutputDescriptor(int deviceId, string outputName) => new InvertedPcfOutput.OutputDescriptor {  DeviceId = deviceId, OutputName = outputName};

        [Fact]
        public void can_enable_disable_specific_bit()
        {
            // Arrange
            var deviceState = 0;
            var i2c = Substitute.For<II2c>();

            i2c.When(x => x.WriteToDevice(Arg.Any<int>(), Arg.Any<byte>()))
               .Do(x => deviceState = x.Arg<byte>());

            var powerOutput = new InvertedPcfOutput(i2c);

            // Act
            powerOutput.TrySetState(BuildOutputDescriptor(1, "O1"), true);  // 1111 1110
            powerOutput.TrySetState(BuildOutputDescriptor(1, "O3"), true);  // 1111 1010
            powerOutput.TrySetState(BuildOutputDescriptor(1, "O6"), true);  // 1101 1010
            powerOutput.TrySetState(BuildOutputDescriptor(1, "O3"), false); // 1101 1110 => 222

            // Assert
            Assert.Equal(222, deviceState);
        }

        [Fact]
        public void can_get_state()
        {
            // Arrange
            var powerOutput = new InvertedPcfOutput(null);

            // Act
            var state = powerOutput.TryGetState(BuildOutputDescriptor(1, "O1"));

            // Assert
            Assert.Equal(false, state);
        }

        [Fact]
        public void can_set_and_read_state()
        {
            // Arrange
            var deviceState = 0;
            var i2c = Substitute.For<II2c>();

            i2c.When(x => x.WriteToDevice(Arg.Any<int>(), Arg.Any<byte>()))
               .Do(x => deviceState = x.Arg<byte>());

            var powerOutput = new InvertedPcfOutput(i2c);

            // Act
            powerOutput.TrySetState(BuildOutputDescriptor(1, "O3"), true);  // 0000 0100

            // Assert
            Assert.Equal(false, powerOutput.TryGetState(BuildOutputDescriptor(1, "O1")));
            Assert.Equal(false, powerOutput.TryGetState(BuildOutputDescriptor(1, "O2")));
            Assert.Equal(true, powerOutput.TryGetState(BuildOutputDescriptor(1, "O3")));
            Assert.Equal(false, powerOutput.TryGetState(BuildOutputDescriptor(1, "O4")));
        }
    }
}
