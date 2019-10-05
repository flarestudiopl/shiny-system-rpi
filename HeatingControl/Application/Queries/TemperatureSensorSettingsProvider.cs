using Domain;
using System.Linq;

namespace HeatingControl.Application.Queries
{
    public interface ITemperatureSensorSettingsProvider
    {
        TemperatureSensorSettings Provide(int temperatureSensorId, Building building);
    }

    public class TemperatureSensorSettings
    {
        public string Name { get; set; }
        public string DeviceId { get; set; }
    }

    public class TemperatureSensorSettingsProvider : ITemperatureSensorSettingsProvider
    {
        public TemperatureSensorSettings Provide(int temperatureSensorId, Building building)
        {
            var temperatureSensor = building.TemperatureSensors.SingleOrDefault(x => x.TemperatureSensorId == temperatureSensorId);

            if (temperatureSensor == null)
            {
                return null;
            }

            return new TemperatureSensorSettings
            {
                Name = temperatureSensor.Name,
                DeviceId = temperatureSensor.DeviceId
            };
        }
    }
}
