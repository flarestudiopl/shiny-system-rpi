using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Domain;
using HeatingControl.Models;

namespace HeatingControl.Application.Queries
{
    public interface INewPowerZoneOptionsProvider
    {
        NewPowerZoneOptionsProviderResult Provide(ControllerState controllerState, Building model);
    }

    public class NewPowerZoneOptionsProviderResult
    {
        public IDictionary<int, string> PowerLimitUnits { get; set; }
        public ICollection<PowerZoneSettingsProviderResult.AffectedHeaterData> Heaters { get; set; }
    }

    public class NewPowerZoneOptionsProvider : INewPowerZoneOptionsProvider
    {
        public NewPowerZoneOptionsProviderResult Provide(ControllerState controllerState, Building model)
        {
            return new NewPowerZoneOptionsProviderResult
                   {
                       PowerLimitUnits = EnumExtensions.AsDictionary<UsageUnit>(),
                       Heaters = controllerState.HeaterIdToState
                                                .Values
                                                .Select(x => new PowerZoneSettingsProviderResult.AffectedHeaterData
                                                             {
                                                                 Id = x.Heater.HeaterId,
                                                                 Name = x.Heater.Name,
                                                                 Assignment = controllerState.PowerZoneIdToState
                                                                                             .Select(z => z.Value.PowerZone)
                                                                                             .Where(z => z.Heaters.Contains(x.Heater))
                                                                                             .Select(z => z.Name).JoinWith(", "),
                                                                 PowerUnit = x.Heater.UsageUnit,
                                                                 OutputState = controllerState.HeaterIdToState[x.Heater.HeaterId].OutputState
                                                             })
                                                .ToList()
                   };
        }
    }
}
