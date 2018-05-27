using Domain.BuildingModel;
using HeatingControl.Application.Commands;
using NSubstitute;
using Storage.BuildingModel;
using System;
using System.Collections.Generic;
using Xunit;

namespace HeatingControl.Tests.Application.Commands
{
    public class NewScheduleItemExecutorTests
    {
        [Theory]
        [InlineData(10, 17)]
        [InlineData(10, 13)]
        [InlineData(10, 18)]
        [InlineData(9, 17)]
        [InlineData(11, 17)]
        [InlineData(9, 11)]
        [InlineData(11, 13)]
        [InlineData(9, 18)]
        public void ignore_when_dates_overlap_existing(int startHour, int endHour)
        {
            //Arrange
            var buildingModelSaver = Substitute.For<IBuildingModelSaver>();

            var schedule = new List<ScheduleItem>
            {
                new ScheduleItem
                {
                    DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday },
                    BeginTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0)
                }
            };

            var building = new Building
            {
                Zones = new List<Zone>
                {
                    new Zone
                    {
                        ZoneId = 1,
                        Schedule = schedule
                    }
                }
            };

            var input = new NewScheduleItemExecutorInput
            {
                ZoneId = 1,
                DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday },
                BeginTime = new TimeSpan(startHour, 0, 0),
                EndTime = new TimeSpan(endHour, 0, 0)
            };

            //Act
            var newScheduleItemExecutor = new NewScheduleItemExecutor(buildingModelSaver);
            newScheduleItemExecutor.Execute(input, building);

            //Assert
            buildingModelSaver.DidNotReceiveWithAnyArgs().Save(null);
            Assert.Equal(1, schedule.Count);
        }
    }
}
