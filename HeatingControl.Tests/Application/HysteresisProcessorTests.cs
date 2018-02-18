using HeatingControl.Application;
using HeatingControl.Models;
using System;
using Xunit;

namespace HeatingControl.Tests.Application
{
    public class HysteresisProcessorTests
    {
        //Arrange
        //Act
        //Assert
        [Theory]
        [InlineData(19.1f, false, false)]
        [InlineData(19.1f, true, true)]
        [InlineData(16.9f, false, true)]
        [InlineData(16.9f, true, true)]
        [InlineData(21.1f, false, false)]
        [InlineData(21.1f, true, false)]
        public void can_turn_heating_on(float currentTemperature, bool currentState, bool expectedResult)
        {
            //Arrange
            var setPoint = 20.0f;
            var hysteresis = 2.0f;

            //Act
            var hysteresisProcessor = new HysteresisProcessor();
            var result = hysteresisProcessor.Process(currentTemperature, currentState, setPoint, hysteresis);

            //Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
