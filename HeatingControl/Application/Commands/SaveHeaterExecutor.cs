using System.Linq;
using Commons;
using Commons.Extensions;
using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public interface ISaveHeaterExecutor
    {
        void Execute(SaveHeaterExecutorInput input, ControllerState controllerState, Building model);
    }

    public class SaveHeaterExecutorInput
    {
        public string Name { get; set; }
        public int PowerOutputDeviceId { get; set; }
        public int PowerOutputChannel { get; set; }
        public UsageUnit UsageUnit { get; set; }
        public decimal UsagePerHour { get; set; }
        public int MinimumStateChangeIntervalSeconds { get; set; }
    }

    public class SaveHeaterExecutor : ISaveHeaterExecutor
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public SaveHeaterExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public void Execute(SaveHeaterExecutorInput input, ControllerState controllerState, Building model)
        {
            if (input.Name.IsNullOrEmpty())
            {
                return;
            }

            if (model.Heaters.Any(x => x.PowerOutputDeviceId == input.PowerOutputDeviceId &&
                                       x.PowerOutputChannel == input.PowerOutputChannel))
            {
                Logger.Warning("Heater with the same power output parameters ({0}/{1}) already exists.",
                               new object[] { input.PowerOutputDeviceId, input.PowerOutputChannel });

                return;
            }

            var heater = new Heater
                         {
                             HeaterId = (controllerState.HeaterIdToState.Keys.Any() ? controllerState.HeaterIdToState.Keys.Max() : 0) + 1,
                             Name = input.Name,
                             PowerOutputDeviceId = input.PowerOutputDeviceId,
                             PowerOutputChannel = input.PowerOutputChannel,
                             UsageUnit = input.UsageUnit,
                             UsagePerHour = input.UsagePerHour,
                             MinimumStateChangeIntervalSeconds = input.MinimumStateChangeIntervalSeconds
                         };

            controllerState.HeaterIdToState.Add(heater.HeaterId, new HeaterState
                                                                 {
                                                                     Heater = heater
                                                                 });

            model.Heaters.Add(heater);
            _buildingModelSaver.Save(model);
        }
    }
}
