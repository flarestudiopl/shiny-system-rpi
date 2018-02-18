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
                ControlLoopIntervalSecondsMilliseconds = 1500,
                Name = "Budynek testowy",
                TemperatureZones = new List<TemperatureZone>
                {
                    new TemperatureZone
                    {
                        TemperatureSensorDeviceId = "10-0008019e9d54",
                        Name = "Strefa 1",
                        AllowedControlTypes = ControlType.ScheduleTemperatureControl | ControlType.ManualTemperatureControl,
                        Heaters = new List<Heater>
                        {
                            new Heater
                            {
                                Name = "Grzejnik A",
                                MinimumStateChangeIntervalSeconds = 1,
                                PowerOutput = new PowerOutputDescriptor{ PowerOutputDeviceId = 60, PowerOutputChannel = 1}
                            }
                        },
                        DefaultSetPoint = 20f,
                        Hysteresis = 0.5f,
                        ManualSetPoint = 22f
                    },
                    new TemperatureZone{ TemperatureSensorDeviceId = "28-000005964edc" },
                    new TemperatureZone{ TemperatureSensorDeviceId = "28-00000595d87e" }
                }
            };
        }
    }
}
