using HeatingControl.Domain;
using System.Collections.Generic;

namespace HeatingControl
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
                Zones = new List<Zone>
                {
                    new Zone{ TemperatureSensorDeviceId = "10-0008019e9d54" },
                    new Zone{ TemperatureSensorDeviceId = "28-000005964edc" },
                    new Zone{ TemperatureSensorDeviceId = "28-00000595d87e" }
                }
            };
        }
    }
}
