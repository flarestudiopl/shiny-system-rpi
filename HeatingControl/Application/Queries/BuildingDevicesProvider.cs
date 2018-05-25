using System.Collections.Generic;
using Domain.BuildingModel;
using HeatingControl.Models;

namespace HeatingControl.Application.Queries
{
    public interface IBuildingDevicesProvider
    {
        BuildingDevicesProviderResult Provide(ControllerState controllerState, Building building);
    }

    public class BuildingDevicesProviderResult
    {
        public string Name { get; set; }
        public ICollection<HeaterData> Heaters { get; set; }
        public ICollection<SensorData> TemperatureSensors { get; set; }
    }

    public class BuildingDevicesProvider : IBuildingDevicesProvider
    {
        private readonly IAvailableDevicesProvider _availableDevicesProvider;

        public BuildingDevicesProvider(IAvailableDevicesProvider availableDevicesProvider)
        {
            _availableDevicesProvider = availableDevicesProvider;
        }

        public BuildingDevicesProviderResult Provide(ControllerState controllerState, Building building)
        {
            var devices = _availableDevicesProvider.Provide(controllerState, building);

            return new BuildingDevicesProviderResult
                   {
                       Name = building.Name,
                       Heaters = devices.Heaters,
                       TemperatureSensors = devices.TemperatureSensors
                   };
        }
    }
}
