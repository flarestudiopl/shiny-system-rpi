using Storage.StorageDatabase;

namespace HeatingControl.Application.DataAccess.TemperatureSensor
{
    public interface ITemperatureSensorSaver
    {
        Domain.TemperatureSensor Save(TemperatureSensorSaverInput input, Domain.Building building);
    }

    public class TemperatureSensorSaverInput
    {
        public int? SensorId { get; set; }
        public string Name { get; set; }
        public string ProtocolName { get; set; }
        public object InputDescriptor { get;set;}
    }

    public class TemperatureSensorSaver : ITemperatureSensorSaver
    {
        private readonly IDbExecutor _dbExecutor;

        public TemperatureSensorSaver(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public Domain.TemperatureSensor Save(TemperatureSensorSaverInput input, Domain.Building building)
        {
            return _dbExecutor.Query(c =>
            {
                c.Attach(building);

                Domain.TemperatureSensor temperatureSensor = null;

                if (input.SensorId.HasValue)
                {
                    temperatureSensor = c.TemperatureSensors.Find(input.SensorId.Value);
                }

                if(temperatureSensor == null)
                {
                    temperatureSensor = new Domain.TemperatureSensor
                    {
                        BuildingId = building.BuildingId
                    };

                    building.TemperatureSensors.Add(temperatureSensor);
                }

                temperatureSensor.Name = input.Name;
                temperatureSensor.ProtocolName = input.ProtocolName;
                temperatureSensor.InputDescriptor = Newtonsoft.Json.JsonConvert.SerializeObject(input.InputDescriptor);

                c.SaveChanges();

                return temperatureSensor;
            });
        }
    }
}
