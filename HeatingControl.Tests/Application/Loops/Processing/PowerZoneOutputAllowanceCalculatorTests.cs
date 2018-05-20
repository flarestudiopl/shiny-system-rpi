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
        public void iterates_over_available_heaters(int iteration, bool state1, bool state2, bool state3, bool state4)
        {
            // Arrange
            var powerZoneState = new PowerZoneState
                                 {
                                     HeaterIdToPowerOnAllowance = new Dictionary<int, bool>
                                                                  {
                                                                      [1] = false,
                                                                      [2] = false,
                                                                      [3] = false,
                                                                      [4] = false
                                                                  },
                                     NextAllowanceRecalculationOffset = iteration,
                                     PowerZone = new PowerZone { MaxUsage = 2.1f }
                                 };

            var heaterState = new HeaterState { Heater = new Heater { UsagePerHour = 0.9f } };

            var controllerState = new ControllerState
                                  {
                                      HeaterIdToState = new Dictionary<int, HeaterState>
                                                        {
                                                            [1] = heaterState,
                                                            [2] = heaterState,
                                                            [3] = heaterState,
                                                            [4] = heaterState
                                                        }
                                  };

            // Act
            var powerZoneOutputAllowanceCalculator = new PowerZoneOutputAllowanceCalculator();
            powerZoneOutputAllowanceCalculator.Calculate(powerZoneState, controllerState);

            // Assert
            Assert.Equal(state1, powerZoneState.HeaterIdToPowerOnAllowance[1]);
            Assert.Equal(state2, powerZoneState.HeaterIdToPowerOnAllowance[2]);
            Assert.Equal(state3, powerZoneState.HeaterIdToPowerOnAllowance[3]);
            Assert.Equal(state4, powerZoneState.HeaterIdToPowerOnAllowance[4]);
        }
    }
}
