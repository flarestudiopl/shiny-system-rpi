using System.Collections.Generic;
using Domain.BuildingModel;
using HeatingControl.Models;

namespace HeatingControl.Application.Queries
{
    public interface INewZoneSettingsProvider
    {
        NewZoneSettingsProviderResult Provide(ControllerState controllerState, Building building);
    }

    public class NewZoneSettingsProviderResult
    {
        public ICollection<SensorData> TemperatureSensors { get; set; }
        public ICollection<HeaterData> Heaters { get; set; }
    }

    public class NewZoneSettingsProvider : INewZoneSettingsProvider
    {
        private readonly IAvailableTemperatureSensorsProvider _availableTemperatureSensorsProvider;
        private readonly IAvailableHeatersProvider _availableHeatersProvider;

        public NewZoneSettingsProvider(IAvailableTemperatureSensorsProvider availableTemperatureSensorsProvider,
                                       IAvailableHeatersProvider availableHeatersProvider)
        {
            _availableTemperatureSensorsProvider = availableTemperatureSensorsProvider;
            _availableHeatersProvider = availableHeatersProvider;
        }

        public NewZoneSettingsProviderResult Provide(ControllerState controllerState, Building building)
        {
            return new NewZoneSettingsProviderResult
                   {
                       TemperatureSensors = _availableTemperatureSensorsProvider.Provide(controllerState, building),
                       Heaters = _availableHeatersProvider.Provide(controllerState)
                   };
        }
    }
}
