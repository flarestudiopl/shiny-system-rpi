using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Domain;
using HeatingControl.Models;

namespace HeatingControl.Application.Queries
{
    public interface IAvailableTemperatureSensorsProvider
    {
        ICollection<SensorData> Provide(ControllerState controllerState, Building building);
    }

    public class SensorData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double? Readout { get; set; }
        public string Assignment { get; set; }
        public string ProtocolName { get;set;}
        public string InputDescriptor { get; set; }
    }

    public class AvailableTemperatureSensorsProvider : IAvailableTemperatureSensorsProvider
    {
        public ICollection<SensorData> Provide(ControllerState controllerState, Building building)
        {
            return controllerState.TemperatureSensorIdToState
                                  .Select(x =>
                                          {
                                              var temperatureSensor = x.Value.TemperatureSensor;

                                              var sensorData = new SensorData
                                                               {
                                                                   Id = x.Key,
                                                                   Name = temperatureSensor.Name,
                                                                   Readout = x.Value.AverageTemperature,
                                                                   ProtocolName = temperatureSensor.ProtocolName,
                                                                   InputDescriptor = temperatureSensor.InputDescriptor
                                                               };

                                              var assignedZones = controllerState.ZoneIdToState
                                                                                 .Select(z => z.Value.Zone)
                                                                                 .Where(z => z.TemperatureControlledZone?.TemperatureSensorId == x.Key)
                                                                                 .Select(z => z.Name)
                                                                                 .ToList();

                                              if (assignedZones.Count > 0)
                                              {
                                                  sensorData.Assignment = assignedZones.JoinWith(", ");
                                              }

                                              return sensorData;
                                          })
                                  .ToList();
        }
    }
}
