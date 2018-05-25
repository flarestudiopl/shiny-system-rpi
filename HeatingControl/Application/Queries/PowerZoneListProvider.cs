using System.Collections.Generic;
using System.Linq;
using Domain.BuildingModel;
using HeatingControl.Models;

namespace HeatingControl.Application.Queries
{
    public interface IPowerZoneListProvider
    {
        ICollection<PowerZoneListItem> Provide(Building model, ControllerState controllerState);
    }

    public class PowerZoneListItem
    {
        public int Id { get; set; }
        public ICollection<string> AffectedHeatersNames { get; set; }
        public float PowerLimitValue { get; set; }
        public UsageUnit PowerLimitUnit { get; set; }
    }

    public class PowerZoneListProvider : IPowerZoneListProvider
    {
        public ICollection<PowerZoneListItem> Provide(Building model, ControllerState controllerState)
        {
            return model.PowerZones.Select(x => new PowerZoneListItem
                                                {
                                                    Id = x.PowerZoneId,
                                                    AffectedHeatersNames = x.HeaterIds.Select(h => controllerState.HeaterIdToState[h].Heater.Name).ToList(),
                                                    PowerLimitUnit = x.UsageUnit,
                                                    PowerLimitValue = x.MaxUsage
                                                })
                        .ToList();
        }
    }
}
