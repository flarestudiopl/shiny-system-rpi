using Commons;
using Domain.BuildingModel;
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
                        ZoneId = 1,
                        Name = "Strefa 1",
                        HeaterIds = new List<int> { 11 },
                        TemperatureControlledZone = new TemperatureControlledZone
                        {
                            TemperatureSensorId = 21,
                            LowSetPoint = 20f,
                            Hysteresis = 0.5f,
                            HighSetPoint = 22f,
                            ScheduleDefaultSetPoint = 15f
                        }
                    },
                    new Zone{
                        ZoneId = 2,
                        Name = "Strefa 2",
                        TemperatureControlledZone = new TemperatureControlledZone
                        {
                            TemperatureSensorId = 22,
                        }
                    },
                    new Zone{
                        ZoneId = 3,
                         TemperatureControlledZone = new TemperatureControlledZone
                         {
                            TemperatureSensorId = 23,
                         },
                        Name = "Strefa 3"
                    }
                },
                Heaters = new List<Heater>
                {
                     new Heater
                     {
                         HeaterId = 11,
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
                        TemperatureSensorId = 21,
                        Name = "Czujnik 1",
                        DeviceId = "10-0008019e9d54"
                    },
                    new TemperatureSensor
                    {
                        TemperatureSensorId = 22,
                        Name = "Czujnik 2",
                        DeviceId = "28-000005964edc"
                    },
                    new TemperatureSensor
                    {
                        TemperatureSensorId = 23,
                        Name = "Czujnik 3",
                        DeviceId = "28-00000595d87e"
                    },
                }
            };
        }
    }
}
