using System.Collections.Generic;
using System.Linq;
using Domain;
using HeatingControl.Models;

namespace HeatingControl.Application.Queries
{
    public interface IAvailableHeatersProvider
    {
        ICollection<HeaterData> Provide(ControllerState controllerState);

        ICollection<HeaterData> Provide(int zoneId, ControllerState controllerState);
    }

    public class HeaterData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Assignment { get; set; }
        public string PowerLimitAssignment { get; set; }
        public bool OutputState { get; set; }
        public decimal UsagePerHour { get; set; }
        public UsageUnit UsageUnit { get; set; }
        public DigitalOutput DigitalOutput { get;set;}
    }

    public class AvailableHeatersProvider : IAvailableHeatersProvider
    {
        public ICollection<HeaterData> Provide(ControllerState controllerState)
        {
            return HeaterDatas(controllerState, null);
        }

        public ICollection<HeaterData> Provide(int zoneId, ControllerState controllerState)
        {
            return HeaterDatas(controllerState, zoneId);
        }

        private static ICollection<HeaterData> HeaterDatas(ControllerState controllerState, int? zoneIdToExcludeFromAssignment)
        {
            return controllerState.HeaterIdToState
                                  .Select(x =>
                                          {
                                              var heaterData = new HeaterData
                                                               {
                                                                   Id = x.Key,
                                                                   Name = x.Value.Heater.Name,
                                                                   OutputState = controllerState.HeaterIdToState[x.Key].OutputState,
                                                                   UsagePerHour = x.Value.Heater.UsagePerHour,
                                                                   UsageUnit = x.Value.Heater.UsageUnit,
                                                                   DigitalOutput = x.Value.Heater.DigitalOutput
                                                               };

                                              if (x.Value.Heater.ZoneId.HasValue)
                                              {
                                                  if (!zoneIdToExcludeFromAssignment.HasValue ||
                                                      x.Value.Heater.ZoneId.Value != zoneIdToExcludeFromAssignment.Value)
                                                  {
                                                      heaterData.Assignment = x.Value.Heater.Zone.Name;
                                                  }
                                              }

                                              if (x.Value.Heater.PowerZoneId.HasValue)
                                              {
                                                  heaterData.PowerLimitAssignment = x.Value.Heater.PowerZone.Name;
                                              }

                                              return heaterData;
                                          })
                                  .OrderBy(x => x.Name)
                                  .ToList();
        }
    }
}
