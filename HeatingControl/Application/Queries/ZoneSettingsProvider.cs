using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Domain;
using HeatingControl.Models;

namespace HeatingControl.Application.Queries
{
    public interface IZoneSettingsProvider
    {
        ZoneSettingsProviderResult Provide(int zoneId, ControllerState controllerState, Building building);
    }

    public class ZoneSettingsProviderResult
    {
        public string Name { get; set; }
        public int? TemperatureSensorId { get; set; }
        public ICollection<SensorData> TemperatureSensors { get; set; }
        public ICollection<int> HeaterIds { get; set; }
        public ICollection<HeaterData> Heaters { get; set; }
    }

    public class ZoneSettingsProvider : IZoneSettingsProvider
    {
        private readonly IAvailableTemperatureSensorsProvider _availableTemperatureSensorsProvider;
        private readonly IAvailableHeatersProvider _availableHeatersProvider;

        public ZoneSettingsProvider(IAvailableTemperatureSensorsProvider availableTemperatureSensorsProvider,
                                    IAvailableHeatersProvider availableHeatersProvider)
        {
            _availableTemperatureSensorsProvider = availableTemperatureSensorsProvider;
            _availableHeatersProvider = availableHeatersProvider;
        }

        public ZoneSettingsProviderResult Provide(int zoneId, ControllerState controllerState, Building building)
        {
            var zone = controllerState.ZoneIdToState.GetValueOrDefault(zoneId);

            if (zone == null)
            {
                return new ZoneSettingsProviderResult();
            }

            var zoneConfiguration = zone.Zone;

            return new ZoneSettingsProviderResult
                   {
                       Name = zoneConfiguration.Name,
                       TemperatureSensorId = zoneConfiguration.TemperatureControlledZone?.TemperatureSensorId,
                       TemperatureSensors = _availableTemperatureSensorsProvider.Provide(controllerState, building),
                       HeaterIds = zoneConfiguration.Heaters.Select(x => x.HeaterId).ToArray(),
                       Heaters = _availableHeatersProvider.Provide(zoneConfiguration.ZoneId, controllerState)
                   };
        }
    }
}
