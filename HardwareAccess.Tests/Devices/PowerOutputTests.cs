﻿using HardwareAccess.Buses;
using HardwareAccess.Devices;
using NSubstitute;
using Xunit;

namespace HardwareAccess.Tests.Devices
{
    public class PowerOutputTests
    {
        [Fact]
        public void can_enable_disable_specific_bit()
        {
            // Arrange
            var deviceState = 0;
            var i2c = Substitute.For<II2c>();

            i2c.When(x => x.WriteToDevice(Arg.Any<int>(), Arg.Any<byte>()))
                .Do(x => deviceState = x.Arg<byte>());

            var powerOutput = new PowerOutput(i2c);

            // Act
            powerOutput.SetState(1, 1, true);  // 0000 0001
            powerOutput.SetState(1, 3, true);  // 0000 0101
            powerOutput.SetState(1, 6, true);  // 0010 0101
            powerOutput.SetState(1, 3, false); // 0010 0001 => 33

            // Assert
            Assert.Equal(33, deviceState);
        }
    }
}
