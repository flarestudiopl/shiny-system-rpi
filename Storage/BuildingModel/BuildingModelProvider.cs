using Commons;
using Domain;
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
                                       HeaterIds = new HashSet<int> { 11 },
                                       TemperatureControlledZone = new TemperatureControlledZone
                                                                   {
                                                                       TemperatureSensorId = 21,
                                                                       LowSetPoint = 20f,
                                                                       Hysteresis = 0.5f,
                                                                       HighSetPoint = 22f,
                                                                       ScheduleDefaultSetPoint = 15f
                                                                   }
                                   },
                                   new Zone
                                   {
                                       ZoneId = 2,
                                       Name = "Strefa 2",
                                       HeaterIds = new HashSet<int> { 12 },
                                       TemperatureControlledZone = new TemperatureControlledZone
                                                                   {
                                                                       TemperatureSensorId = 22,
                                                                       LowSetPoint = 20f,
                                                                       Hysteresis = 0.5f,
                                                                       HighSetPoint = 22f,
                                                                       ScheduleDefaultSetPoint = 15f
                                                                   }
                                   },
                                   new Zone
                                   {
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
                                         PowerOutputDeviceId = 56,
                                         PowerOutputChannel = "O5",
                                         PowerOutputProtocolName = ProtocolNames.InvertedPcf,
                                         UsagePerHour = 2,
                                         UsageUnit = UsageUnit.kW
                                     },
                                     new Heater
                                     {
                                         HeaterId = 12,
                                         Name = "Grzejnik B",
                                         MinimumStateChangeIntervalSeconds = 1,
                                         PowerOutputDeviceId = 56,
                                         PowerOutputChannel = "O6",
                                         PowerOutputProtocolName = ProtocolNames.InvertedPcf,
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
                                            },
                       PowerZones = new List<PowerZone>
                                    {
                                        new PowerZone
                                        {
                                            Name = "Max 3kW",
                                            MaxUsage = 3m,
                                            UsageUnit = UsageUnit.kW,
                                            HeaterIds = new HashSet<int> { 11, 12 },
                                            PowerZoneId = 1200,
                                            RoundRobinIntervalMinutes = 1
                                        }
                                    }
                   };
        }
    }
}
