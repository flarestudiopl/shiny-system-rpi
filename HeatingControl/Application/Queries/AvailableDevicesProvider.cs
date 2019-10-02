using System.Collections.Generic;
using Domain;
using HeatingControl.Models;

namespace HeatingControl.Application.Queries
{
    public interface IAvailableDevicesProvider
    {
        AvailableDevicesProviderResult Provide(ControllerState controllerState, Building building);
    }

    public class AvailableDevicesProviderResult
    {
        public ICollection<SensorData> TemperatureSensors { get; set; }
        public ICollection<HeaterData> Heaters { get; set; }
    }

    public class AvailableDevicesProvider : IAvailableDevicesProvider
    {
        private readonly IAvailableTemperatureSensorsProvider _availableTemperatureSensorsProvider;
        private readonly IAvailableHeatersProvider _availableHeatersProvider;

        public AvailableDevicesProvider(IAvailableTemperatureSensorsProvider availableTemperatureSensorsProvider,
                                        IAvailableHeatersProvider availableHeatersProvider)
        {
            _availableTemperatureSensorsProvider = availableTemperatureSensorsProvider;
            _availableHeatersProvider = availableHeatersProvider;
        }

        public AvailableDevicesProviderResult Provide(ControllerState controllerState, Building building)
        {
            return new AvailableDevicesProviderResult
                   {
                       TemperatureSensors = _availableTemperatureSensorsProvider.Provide(controllerState, building),
                       Heaters = _availableHeatersProvider.Provide(controllerState)
                   };
        }
    }
}
