using System.Collections.Generic;
using System.Linq;
using Domain;
using HardwareAccess.Devices;

namespace HeatingControl.Application.Queries
{
    public interface IConnectedTemperatureSensorsProvider
    {
        ICollection<ConnectedTemperatureSensor> Provide(Building model);
    }

    public class ConnectedTemperatureSensor
    {
        public string DeviceId { get; set; }
        public string AssignedTemperatureSensorName { get; set; }
    }

    public class ConnectedTemperatureSensorsProvider : IConnectedTemperatureSensorsProvider
    {
        private readonly ITemperatureSensor _temperatureSensor;

        public ConnectedTemperatureSensorsProvider(ITemperatureSensor temperatureSensor)
        {
            _temperatureSensor = temperatureSensor;
        }

        public ICollection<ConnectedTemperatureSensor> Provide(Building model)
        {
            var allTemperatureSensors = _temperatureSensor.GetAvailableSensors();

            return allTemperatureSensors.Select(x =>
                                                {
                                                    var modelSensor = model.TemperatureSensors.FirstOrDefault(s => s.DeviceId == x);

                                                    return new ConnectedTemperatureSensor
                                                           {
                                                               DeviceId = x,
                                                               AssignedTemperatureSensorName = modelSensor?.Name
                                                           };
                                                }).ToList();
        }
    }
}
