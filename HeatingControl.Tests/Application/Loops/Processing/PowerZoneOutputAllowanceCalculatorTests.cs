using System.Collections.Generic;
using Domain.BuildingModel;
using HeatingControl.Application.Loops.Processing;
using HeatingControl.Models;
using Xunit;

namespace HeatingControl.Tests.Application.Loops.Processing
{
    public class PowerZoneOutputAllowanceCalculatorTests
    {
        [Theory]
        [InlineData(0, true, true, false, false)]
        [InlineData(1, false, true, true, false)]
        [InlineData(2, false, false, true, true)]
        [InlineData(3, true, false, false, true)]
        [InlineData(4, true, true, false, false)]
        public void iterates_over_available_heaters(byte iteration, bool state1, bool state2, bool state3, bool state4)
        {
            // Arrange
            var powerZoneState = new PowerZoneState
                                 {
                                     NextIntervalOffset = iteration,
                                     PowerZone = new PowerZone
                                                 {
                                                     MaxUsage = 2.1m,
                                                     HeaterIds = new HashSet<int> { 1, 2, 3, 4 }
                                                 }
                                 };

            var heater = new Heater { UsagePerHour = 0.9m };
            var heaterState1 = new HeaterState { Heater = heater, OutputState = true };
            var heaterState2 = new HeaterState { Heater = heater, OutputState = true };
            var heaterState3 = new HeaterState { Heater = heater, OutputState = true };
            var heaterState4 = new HeaterState { Heater = heater, OutputState = true };

            var controllerState = new ControllerState
                                  {
                                      HeaterIdToState = new Dictionary<int, HeaterState>
                                                        {
                                                            [1] = heaterState1,
                                                            [2] = heaterState2,
                                                            [3] = heaterState3,
                                                            [4] = heaterState4
                                                        }
                                  };

            // Act
            var powerZoneOutputAllowanceCalculator = new PowerZoneOutputLimiter();
            powerZoneOutputAllowanceCalculator.Limit(powerZoneState, controllerState);

            // Assert
            Assert.Equal(state1, heaterState1.OutputState);
            Assert.Equal(state2, heaterState2.OutputState);
            Assert.Equal(state3, heaterState3.OutputState);
            Assert.Equal(state4, heaterState4.OutputState);
        }
    }
}
