﻿using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Domain;
using HeatingControl.Models;

namespace HeatingControl.Application.Queries
{
    public interface IPowerZoneSettingsProvider
    {
        PowerZoneSettingsProviderResult Provide(int powerZoneId, ControllerState controllerState);
    }

    public class PowerZoneSettingsProviderResult
    {
        public string Name { get; set; }
        public decimal PowerLimitValue { get; set; }
        public UsageUnit PowerLimitUnit { get; set; }
        public IDictionary<int, string> PowerLimitUnits { get; set; }
        public ICollection<int> AffectedHeatersIds { get; set; }
        public ICollection<AffectedHeaterData> Heaters { get; set; }
        public int RoundRobinIntervalMinutes { get; set; }
        public int SwitchDelay { get; set; }

        public class AffectedHeaterData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Assignment { get; set; }
            public UsageUnit PowerUnit { get; set; }
            public bool OutputState { get; set; }
        }
    }

    public class PowerZoneSettingsProvider : IPowerZoneSettingsProvider
    {
        public PowerZoneSettingsProviderResult Provide(int powerZoneId, ControllerState controllerState)
        {
            var powerZone = controllerState.PowerZoneIdToState.GetValueOrDefault(powerZoneId);

            if (powerZone == null)
            {
                return null;
            }

            var powerZoneModel = powerZone.PowerZone;

            return new PowerZoneSettingsProviderResult
                   {
                       Name = powerZoneModel.Name,
                       PowerLimitValue = powerZoneModel.MaxUsage,
                       PowerLimitUnit = powerZoneModel.UsageUnit,
                       PowerLimitUnits = EnumExtensions.AsDictionary<UsageUnit>(),
                       AffectedHeatersIds = powerZoneModel.Heaters.Select(x => x.HeaterId).ToArray(),
                       Heaters = controllerState.HeaterIdToState
                                                .Values
                                                .Select(x => new PowerZoneSettingsProviderResult.AffectedHeaterData
                                                             {
                                                                 Id = x.Heater.HeaterId,
                                                                 Name = x.Heater.Name,
                                                                 Assignment = x.Heater.PowerZone == powerZoneModel ? null : x.Heater.PowerZone?.Name,
                                                                 PowerUnit = x.Heater.UsageUnit,
                                                                 OutputState = controllerState.HeaterIdToState[x.Heater.HeaterId].OutputState
                                                             })
                                                .ToList(),
                       RoundRobinIntervalMinutes = powerZoneModel.RoundRobinIntervalMinutes,
                       SwitchDelay = powerZoneModel.SwitchDelayBetweenOutputsSeconds
                  };
        }
    }
}
