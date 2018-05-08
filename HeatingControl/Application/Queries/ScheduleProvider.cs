using System.Collections.Generic;
using HeatingControl.Domain;
using HeatingControl.Models;

namespace HeatingControl.Application.Queries
{
    public interface IScheduleProvider
    {
        ScheduleProviderResult Provide(int zoneId, ControllerState controllerState);
    }

    public class ScheduleProviderResult
    {
        public ICollection<ScheduleItem> Schedule { get; set; }
    }

    public class ScheduleProvider : IScheduleProvider
    {
        public ScheduleProviderResult Provide(int zoneId, ControllerState controllerState)
        {
            var zone = controllerState.ZoneIdToState.GetValueOrDefault(zoneId);

            if (zone == null)
            {
                return new ScheduleProviderResult();
            }

            return new ScheduleProviderResult
                   {
                       Schedule = zone.Zone.Schedule
                   };
        }
    }
}
