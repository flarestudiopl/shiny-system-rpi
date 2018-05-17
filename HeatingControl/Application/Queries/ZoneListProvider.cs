﻿using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using HeatingControl.Application.Loops.Processing;
using Domain.BuildingModel;
using HeatingControl.Models;

namespace HeatingControl.Application.Queries
{
    public interface IZoneListProvider
    {
        ZoneListProviderOutput Provide(ControllerState state, Building building);
    }

    public class ZoneListProviderOutput
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

        public ZoneListProviderOutput Provide(ControllerState state, Building building)
        {
            return new ZoneListProviderOutput
                   {
                       Zones = state.ZoneIdToState
                                    .Values
                                    .Select(x =>
                                            {
                                                var zone = x.Zone;

                                                return new ZoneListProviderOutput.ZoneListItem
                                                       {
                                                           Id = zone.ZoneId,
                                                           Name = zone.Name,
                                                           HeaterNames = zone
                                                                        .HeaterIds
                                                                        .Select(h => state.HeaterIdToState[h].Heater.Name)
                                                                        .ToList(),
                                                           TemperatureSensorName = building.TemperatureSensors
                                                                                           .First(t => t.TemperatureSensorId == zone.TemperatureControlledZone.TemperatureSensorId)
                                                                                           .Name,
                                                           TotalPowerFormatted = _zonePowerProvider.Provide(zone.ZoneId, state)
                                                                                                   .Select(h => $"{h.Value} {Enum.GetName(typeof(UsageUnit), h.Key)}")
                                                                                                   .JoinWith(", ")
                                                       };
                                            }).ToList()
                   };
        }
    }
}