using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Domain.BuildingModel;
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
        public float PowerLimitValue { get; set; }
        public UsageUnit PowerLimitUnit { get; set; }
        public IDictionary<int, string> PowerLimitUnits { get; set; }
        public ICollection<int> AffectedHeatersIds { get; set; }
        public ICollection<AffectedHeaterData> Heaters { get; set; }
        public int RoundRobinIntervalMinutes { get; set; }

        public class AffectedHeaterData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Assignment { get; set; }
            public UsageUnit PowerUnit { get; set; }
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
                       AffectedHeatersIds = powerZoneModel.HeaterIds,
                       Heaters = controllerState.HeaterIdToState
                                                .Values
                                                .Select(x => new PowerZoneSettingsProviderResult.AffectedHeaterData
                                                             {
                                                                 Id = x.Heater.HeaterId,
                                                                 Name = x.Heater.Name,
                                                                 Assignment = controllerState.PowerZoneIdToState
                                                                                             .Select(z => z.Value.PowerZone)
                                                                                             .Where(z => z.HeaterIds.Contains(x.Heater.HeaterId) &&
                                                                                                         z.PowerZoneId != powerZoneId)
                                                                                             .Select(z => z.Name).JoinWith(", "),
                                                                 PowerUnit = x.Heater.UsageUnit
                                                             })
                                                .ToList(),
                       RoundRobinIntervalMinutes = powerZoneModel.RoundRobinIntervalMinutes
                   };
        }
    }
}
