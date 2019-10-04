using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Domain;
using HeatingControl.Models;

namespace HeatingControl.Application.Loops.Processing
{
    public interface IZonePowerProvider
    {
        IDictionary<UsageUnit, decimal> Provide(int zoneId, ControllerState controllerState);
    }

    public class ZonePowerProvider : IZonePowerProvider
    {
        public IDictionary<UsageUnit, decimal> Provide(int zoneId, ControllerState controllerState)
        {
            var zone = controllerState.ZoneIdToState.GetValueOrDefault(zoneId);

            return zone?.Zone
                        .Heaters
                        .GroupBy(x => x.UsageUnit)
                        .ToDictionary(x => x.Key, x => x.Sum(u => u.UsagePerHour));
        }
    }
}
