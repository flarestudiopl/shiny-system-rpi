using System.Collections.Generic;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    /// <summary>
    /// Controller for settings views
    /// </summary>
    [Route("/api/setup/device")]
    public class DeviceSetupController : BaseController
    {
        private readonly IHeatingControl _heatingControl;
        private readonly IBuildingDevicesProvider _buildingDevicesProvider;
        private readonly IUpdateBuildingExecutor _updateBuildingExecutor;
        private readonly IConnectedTemperatureSensorsProvider _connectedTemperatureSensorsProvider;
        private readonly ISaveTemperatureSensorExecutor _saveTemperatureSensorExecutor;
        private readonly IRemoveTemperatureSensorExecutor _removeTemperatureSensorExecutor;
        private readonly INewHeaterOptionsProvider _newHeaterOptionsProvider;
        private readonly ISaveHeaterExecutor _saveHeaterExecutor;
        private readonly IRemoveHeaterExecutor _removeHeaterExecutor;

        public DeviceSetupController(IHeatingControl heatingControl,
                                     IBuildingDevicesProvider buildingDevicesProvider,
                                     IUpdateBuildingExecutor updateBuildingExecutor,
                                     IConnectedTemperatureSensorsProvider connectedTemperatureSensorsProvider,
                                     ISaveTemperatureSensorExecutor saveTemperatureSensorExecutor,
                                     IRemoveTemperatureSensorExecutor removeTemperatureSensorExecutor,
                                     INewHeaterOptionsProvider newHeaterOptionsProvider,
                                     ISaveHeaterExecutor saveHeaterExecutor,
                                     IRemoveHeaterExecutor removeHeaterExecutor)
        {
            _heatingControl = heatingControl;
            _buildingDevicesProvider = buildingDevicesProvider;
            _updateBuildingExecutor = updateBuildingExecutor;
            _connectedTemperatureSensorsProvider = connectedTemperatureSensorsProvider;
            _saveTemperatureSensorExecutor = saveTemperatureSensorExecutor;
            _removeTemperatureSensorExecutor = removeTemperatureSensorExecutor;
            _newHeaterOptionsProvider = newHeaterOptionsProvider;
            _saveHeaterExecutor = saveHeaterExecutor;
            _removeHeaterExecutor = removeHeaterExecutor;
        }

        [HttpGet]
        public BuildingDevicesProviderResult GetDeviceConfiguration()
        {
            return _buildingDevicesProvider.Provide(_heatingControl.State, _heatingControl.State.Model);
        }

        [HttpPost("buildingName/{name}")]
        public void SetBuildingName(string name)
        {
            _updateBuildingExecutor.Execute(new UpdateBuildingExecutorInput
                                            {
                                                Name = name
                                            },
                                            _heatingControl.State.Model);
        }

        [HttpGet("connectedTemperatureSensors")]
        public ICollection<ConnectedTemperatureSensor> GetConnectedTemperetureSensors()
        {
            return _connectedTemperatureSensorsProvider.Provide(_heatingControl.State.Model);
        }

        [HttpPost("temperatureSensor")]
        public void AddTemperatureSensor([FromBody] SaveTemperatureSensorExecutorInput input)
        {
            _saveTemperatureSensorExecutor.Execute(input, _heatingControl.State.Model, _heatingControl.State);
        }

        [HttpDelete("temperatureSensor/{temperatureSensorId}")]
        public void RemoveTemperatureSensor(int temperatureSensorId)
        {
            _removeTemperatureSensorExecutor.Execute(temperatureSensorId, _heatingControl.State, _heatingControl.State.Model);
        }

        [HttpGet("heater/new")]
        public NewHeaterOptionsProviderResult GetNewHeaterOptions()
        {
            return _newHeaterOptionsProvider.Provide();
        }

        [HttpPost("heater")]
        public void AddHeater([FromBody] SaveHeaterExecutorInput input)
        {
            _saveHeaterExecutor.Execute(input, _heatingControl.State, _heatingControl.State.Model);
        }

        [HttpDelete("heater/{heaterId}")]
        public void RemoveHeater(int heaterId)
        {
            _removeHeaterExecutor.Execute(heaterId, _heatingControl.State, _heatingControl.State.Model);
        }
    }
}
