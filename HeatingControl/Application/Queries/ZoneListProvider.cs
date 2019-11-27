using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using HeatingControl.Application.Loops.Processing;
using HeatingControl.Models;
using Domain;

namespace HeatingControl.Application.Queries
{
    public interface IZoneListProvider
    {
        ZoneListProviderResult Provide(ControllerState state, Building building);
    }

    public class ZoneListProviderResult
    {
        public ICollection<ZoneListItem> Zones { get; set; }

        public class ZoneListItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string TemperatureSensorName { get; set; }
            public ICollection<string> HeaterNames { get; set; }
            public string TotalPowerFormatted { get; set; }
        }
    }

    public class ZoneListProvider : IZoneListProvider
    {
        private readonly IZonePowerProvider _zonePowerProvider;

        public ZoneListProvider(IZonePowerProvider zonePowerProvider)
        {
            _zonePowerProvider = zonePowerProvider;
        }

        public ZoneListProviderResult Provide(ControllerState state, Building building)
        {
            return new ZoneListProviderResult
                   {
                       Zones = state.ZoneIdToState
                                    .Values
                                    .Select(x =>
                                            {
                                                var zone = x.Zone;

                                                var zoneListItem = new ZoneListProviderResult.ZoneListItem
                                                                   {
                                                                       Id = zone.ZoneId,
                                                                       Name = zone.Name,
                                                                       HeaterNames = zone
                                                                                    .Heaters
                                                                                    .Select(h => h.Name)
                                                                                    .ToList(),
                                                                       TotalPowerFormatted = _zonePowerProvider.Provide(zone.ZoneId, state)
                                                                                                               .Select(h => $"{h.Value} {Enum.GetName(typeof(UsageUnit), h.Key)}")
                                                                                                               .JoinWith(", ")
                                                                   };

                                                if (zone.TemperatureControlledZone != null)
                                                {
                                                    zoneListItem.TemperatureSensorName = building.TemperatureSensors
                                                                                                 .First(t => t.TemperatureSensorId == zone.TemperatureControlledZone.TemperatureSensorId)
                                                                                                 .Name;
                                                }

                                                return zoneListItem;
                                            })
                                    .OrderBy(x => x.Name)
                                    .ToList()
                   };
        }
    }
}
