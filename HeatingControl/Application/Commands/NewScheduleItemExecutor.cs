using System;
using System.Collections.Generic;
using System.Linq;
using Commons;
using Domain.BuildingModel;
using HeatingControl.Extensions;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public interface INewScheduleItemExecutor
    {
        void Execute(NewScheduleItemExecutorInput input, Building building);
    }

    public class NewScheduleItemExecutorInput
    {
        public int ZoneId { get; set; }
        public ICollection<DayOfWeek> DaysOfWeek { get; set; }
        public TimeSpan BeginTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public float? SetPoint { get; set; }
    }

    public class NewScheduleItemExecutor : INewScheduleItemExecutor
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public NewScheduleItemExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public void Execute(NewScheduleItemExecutorInput input, Building building)
        {
            if (input.BeginTime >= input.EndTime)
            {
                Logger.Warning("{0} should be greater than {1}.", new object[] { nameof(input.EndTime), nameof(input.BeginTime) });

                return;
            }

            var zone = building.Zones.FirstOrDefault(x => x.ZoneId == input.ZoneId);

            if (zone == null)
            {
                return;
            }

            if (zone.IsTemperatureControlled() && !input.SetPoint.HasValue)
            {
                Logger.Warning("{0} should be set for temperature controlled zone.", new object[] { nameof(input.SetPoint) });

                return;
            }

            if (zone.Schedule.Any(x => x.DaysOfWeek.Any(d => input.DaysOfWeek.Contains(d) &&
                                                             (input.BeginTime >= x.BeginTime && input.BeginTime < x.EndTime ||
                                                              input.EndTime > x.BeginTime && input.EndTime <= x.EndTime))))
            {
                Logger.Warning("Given schedule parameters overlaps existing item.");

                return;
            }

            var lastScheduleItem = zone.Schedule.OrderByDescending(x => x.ScheduleItemId).FirstOrDefault();

            var newScheduleItem = new ScheduleItem
                                  {
                                      ScheduleItemId = (lastScheduleItem?.ScheduleItemId ?? 0) + 1,
                                      DaysOfWeek = input.DaysOfWeek,
                                      BeginTime = input.BeginTime,
                                      EndTime = input.EndTime,
                                      SetPoint = input.SetPoint
                                  };

            zone.Schedule.Add(newScheduleItem);

            _buildingModelSaver.Save(building);
        }
    }
}
