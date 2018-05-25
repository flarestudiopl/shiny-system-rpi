using System.Linq;
using Commons.Extensions;
using Domain.BuildingModel;
using HeatingControl.Models;
using Storage.BuildingModel;

namespace HeatingControl.Application.Commands
{
    public interface ISaveTemperatureSensorExecutor
    {
        void Execute(SaveTemperatureSensorExecutorInput input, Building model, ControllerState controllerState);
    }

    public class SaveTemperatureSensorExecutorInput
    {
        public string Name { get; set; }
        public string DeviceId { get; set; }
    }

    public class SaveTemperatureSensorExecutor : ISaveTemperatureSensorExecutor
    {
        private readonly IBuildingModelSaver _buildingModelSaver;

        public SaveTemperatureSensorExecutor(IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelSaver = buildingModelSaver;
        }

        public void Execute(SaveTemperatureSensorExecutorInput input, Building model, ControllerState controllerState)
        {
            if (input.Name.IsNullOrEmpty() || input.DeviceId.IsNullOrEmpty())
            {
                return;
            }

            if (model.TemperatureSensors.Any(x => x.DeviceId == input.DeviceId))
            {
                return;
            }

            if (!controllerState.TemperatureDeviceIdToTemperatureData.ContainsKey(input.DeviceId))
            {
                controllerState.TemperatureDeviceIdToTemperatureData.Add(input.DeviceId, new TemperatureData());
            }

            var temperatureSensor = new TemperatureSensor
                                    {
                                        TemperatureSensorId = (controllerState.TemperatureSensorIdToDeviceId.Keys.Any() ? controllerState.TemperatureSensorIdToDeviceId.Keys.Max() : 0) + 1,
                                        Name = input.Name,
                                        DeviceId = input.DeviceId
                                    };

            controllerState.TemperatureSensorIdToDeviceId.Add(temperatureSensor.TemperatureSensorId, input.DeviceId);

            model.TemperatureSensors.Add(temperatureSensor);

            _buildingModelSaver.Save(model);
        }
    }
}
