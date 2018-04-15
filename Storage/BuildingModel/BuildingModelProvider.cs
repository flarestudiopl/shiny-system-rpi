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
                Zones = new List<Zone>
                {
                    new Zone
                    {
                        Name = "Strefa 1",
                        HeatersNames = new List<string> { "Grzejnik A" },
                        TemperatureControlledZone = new TemperatureControlledZone
                        {
                            TemperatureSensorName = "Czujnik 1",
                            LowSetPoint = 20f,
                            Hysteresis = 0.5f,
                            HighSetPoint = 22f,
                            ScheduleDefaultSetPoint = 15f
                        }
                    },
                    new Zone{
                        Name = "Strefa 2",
                        TemperatureControlledZone = new TemperatureControlledZone
                        {
                            TemperatureSensorName = "Czujnik 2"
                        }
                    },
                    new Zone{
                         TemperatureControlledZone = new TemperatureControlledZone
                         {
                            TemperatureSensorName = "Czujnik 3"
                         },
                        Name = "Strefa 3"
                    }
                },
                Heaters = new List<Heater>
                {
                     new Heater
                     {
                         Name = "Grzejnik A",
                         MinimumStateChangeIntervalSeconds = 1,
                         PowerOutput = new PowerOutput{ PowerOutputDeviceId = 60, PowerOutputChannel = 1},
                         UsagePerHour = 2,
                         UsageUnit = UsageUnit.kW
                     }
                },
                TemperatureSensors = new List<TemperatureSensor>
                {
                    new TemperatureSensor
                    {
                        Name = "Czujnik 1",
                        DeviceId = "10-0008019e9d54"
                    },
                    new TemperatureSensor
                    {
                        Name = "Czujnik 2",
                        DeviceId = "28-000005964edc"
                    },
                    new TemperatureSensor
                    {
                        Name = "Czujnik 3",
                        DeviceId = "28-00000595d87e"
                    },
                }
            };
        }
    }
}
