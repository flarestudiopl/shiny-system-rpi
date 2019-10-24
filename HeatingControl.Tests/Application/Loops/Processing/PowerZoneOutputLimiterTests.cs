using System.Collections.Generic;
using Domain;
using HeatingControl.Application.Loops.Processing;
using HeatingControl.Models;
using Xunit;

namespace HeatingControl.Tests.Application.Loops.Processing
{
    public class PowerZoneOutputLimiterTests
    {
        [Theory]
        [InlineData(255, true, true, false, false)]
        [InlineData(0, false, true, true, false)]
        [InlineData(1, false, false, true, true)]
        [InlineData(2, true, false, false, true)]
        [InlineData(3, true, true, false, false)]
        [InlineData(4, false, true, true, false)]
        public void iterates_over_available_heaters(byte iteration, bool state1, bool state2, bool state3, bool state4)
        {
            // Arrange
            var heater1 = new Heater { HeaterId = 1, UsagePerHour = 0.9m };
            var heater2 = new Heater { HeaterId = 2, UsagePerHour = 0.9m };
            var heater3 = new Heater { HeaterId = 3, UsagePerHour = 0.9m };
            var heater4 = new Heater { HeaterId = 4, UsagePerHour = 0.9m };

            var heaterState1 = new HeaterState { Heater = heater1, OutputState = true };
            var heaterState2 = new HeaterState { Heater = heater2, OutputState = true };
            var heaterState3 = new HeaterState { Heater = heater3, OutputState = true };
            var heaterState4 = new HeaterState { Heater = heater4, OutputState = true };

            var powerZoneState = new PowerZoneState
            {
                NextIntervalOffset = iteration,
                PowerZone = new PowerZone
                {
                    MaxUsage = 2.1m,
                    Heaters = new List<Heater> { heater1, heater2, heater3, heater4 }
                }
            };

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
