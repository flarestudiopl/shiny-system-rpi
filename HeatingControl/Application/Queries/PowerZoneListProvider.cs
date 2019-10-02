using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using HeatingControl.Models;

namespace HeatingControl.Application.Queries
{
    public interface IPowerZoneListProvider
    {
        PowerZoneListProviderResult Provide(Building model, ControllerState controllerState);
    }

    public class PowerZoneListProviderResult
    {
        public ICollection<PowerZoneListItem> PowerZones { get; set; }

        public class PowerZoneListItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public ICollection<string> AffectedHeatersNames { get; set; }
            public string PowerLimitFormatted { get; set; }
        }
    }

    public class PowerZoneListProvider : IPowerZoneListProvider
    {
        public PowerZoneListProviderResult Provide(Building model, ControllerState controllerState)
        {
            return new PowerZoneListProviderResult
                   {
                       PowerZones = model.PowerZones
                                         .Select(x => new PowerZoneListProviderResult.PowerZoneListItem
                                                      {
                                                          Id = x.PowerZoneId,
                                                          Name = x.Name,
                                                          AffectedHeatersNames = x.Heaters.Select(h => h.Name).ToList(),
                                                          PowerLimitFormatted = $"{x.MaxUsage} {Enum.GetName(typeof(UsageUnit), x.UsageUnit)}"
                                                      })
                                         .OrderBy(x => x.Name)
                                         .ToList()
                   };
        }
    }
}
