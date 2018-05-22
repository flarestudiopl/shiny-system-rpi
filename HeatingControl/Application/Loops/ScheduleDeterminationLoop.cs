using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Domain.BuildingModel;
using HeatingControl.Extensions;
using HeatingControl.Models;

namespace HeatingControl.Application.Loops
{
    public interface IScheduleDeterminationLoop
    {
        void Start(int intervalMilliseconds, ControllerState controllerState, CancellationToken cancellationToken);
    }

    public class ScheduleDeterminationLoop : IScheduleDeterminationLoop
    {
        public void Start(int intervalMilliseconds, ControllerState controllerState, CancellationToken cancellationToken)
        {
            Loop.Start("Schedule determination",
                       intervalMilliseconds,
                       () => ProcessSchedule(controllerState),
                       cancellationToken);
        }

        private static void ProcessSchedule(ControllerState controllerState)
        {
            var currentTime = DateTime.Now;

            foreach (var zoneState in controllerState.ZoneIdToState.Values)
            {
                var currentScheduleItem = TryGetScheduleItem(zoneState.Zone.Schedule, currentTime);

                if (zoneState.Zone.IsTemperatureControlled())
                {
                    ProcessTemperatureControlledSchedule(currentScheduleItem, zoneState);
                }
                else
                {
                    ProcessOnOffControlledSchedule(currentScheduleItem, zoneState);
                }
            }
        }

        private static void ProcessTemperatureControlledSchedule(ScheduleItem currentScheduleItem, ZoneState zoneState)
        {
            zoneState.ScheduleState.HeatingEnabled = null;

            zoneState.ScheduleState.DesiredTemperature = currentScheduleItem != null
                                                             ? currentScheduleItem.SetPoint
                                                             : zoneState.Zone.TemperatureControlledZone.ScheduleDefaultSetPoint;
        }

        private static void ProcessOnOffControlledSchedule(ScheduleItem currentScheduleItem, ZoneState zoneState)
        {
            zoneState.ScheduleState.DesiredTemperature = null;

            zoneState.ScheduleState.HeatingEnabled = currentScheduleItem != null;
        }

        private static ScheduleItem TryGetScheduleItem(ICollection<ScheduleItem> scheduleItems, DateTime now)
        {
            if (scheduleItems != null && scheduleItems.Any())
            {
                return scheduleItems.FirstOrDefault(x => x.DaysOfWeek.Contains(now.DayOfWeek) &&
                                                         x.BeginTime < now.TimeOfDay &&
                                                         x.EndTime >= now.TimeOfDay);
            }

            return null;
        }
    }
}
