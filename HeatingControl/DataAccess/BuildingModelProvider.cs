using HeatingControl.Domain;
using System.Collections.Generic;

namespace HeatingControl.DataAccess
{
    public interface IBuildingModelProvider
    {
        Building Provide();
    }

    public class BuildingModelProvider : IBuildingModelProvider
    {
        public Building Provide()
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
