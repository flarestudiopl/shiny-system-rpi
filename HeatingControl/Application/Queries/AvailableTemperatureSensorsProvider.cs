﻿using System.Collections.Generic;
using System.Linq;
using HeatingControl.Domain;
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
        public float Readout { get; set; }
    }

    public class AvailableTemperatureSensorsProvider : IAvailableTemperatureSensorsProvider
    {
        public ICollection<SensorData> Provide(ControllerState controllerState, Building building)
        {
            return controllerState.TemperatureSensorIdToDeviceId
                                  .Select(x =>
                                          {
                                              var sensorData = new SensorData
                                                               {
                                                                   Id = x.Key
                                                               };

                                              var temperatureData = controllerState.TemperatureDeviceIdToTemperatureData.GetValueOrDefault(x.Value);

                                              if (temperatureData != null)
                                              {
                                                  sensorData.Readout = temperatureData.AverageTemperature;
                                              }

                                              var sensorConfiguration = building.TemperatureSensors.FirstOrDefault(ts => ts.TemperatureSensorId == x.Key);

                                              if (sensorConfiguration != null)
                                              {
                                                  sensorData.Name = sensorConfiguration.Name;
                                              }

                                              return sensorData;
                                          })
                                  .ToList();
        }
    }
}