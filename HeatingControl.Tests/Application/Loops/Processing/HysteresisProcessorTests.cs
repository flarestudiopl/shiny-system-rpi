using HeatingControl.Application.Loops.Processing;
using Xunit;

namespace HeatingControl.Tests.Application.Loops.Processing
{
    public class HysteresisProcessorTests
    {
        [Theory]
        [InlineData(16.90d, false, true)]
        [InlineData(16.90d, true, true)]
        [InlineData(19.10d, false, false)]
        [InlineData(19.10d, true, true)]
        [InlineData(21.10d, false, false)]
        [InlineData(21.10d, true, false)]
        public void keep_state_inside_hysteresis_loop(double currentTemperature, bool currentState, bool expectedResult)
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
