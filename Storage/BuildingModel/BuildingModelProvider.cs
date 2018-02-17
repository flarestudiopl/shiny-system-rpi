using Commons;
using HeatingControl.Domain;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Storage.BuildingModel
{
    public interface IBuildingModelProvider
    {
        Building Provide();
    }

    public class BuildingModelProvider : IBuildingModelProvider
    {
        private readonly IConfiguration _configuration;

        public BuildingModelProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Building Provide()
        {
            var buildingModelFilePath = _configuration[BuildingModelSaver.BuildingModelConfigPath];
            var building = JsonFile.Read<Building>(buildingModelFilePath);

            if (building == null)
            {
                building = CreateSampleModel();
            }

            return building;
        }

        private static Building CreateSampleModel()
        {
            return new Building
            {
                TemperatureZones = new List<TemperatureZone>
                {
                    new TemperatureZone{ TemperatureSensorDeviceId = "10-0008019e9d54" },
                    new TemperatureZone{ TemperatureSensorDeviceId = "28-000005964edc" },
                    new TemperatureZone{ TemperatureSensorDeviceId = "28-00000595d87e" }
                }
            };
        }
    }
}
