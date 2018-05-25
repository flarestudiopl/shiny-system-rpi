using System.Collections.Generic;
using System.Threading.Tasks;
using HardwareAccess.Buses;
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
        private readonly II2c _i2C;
        private readonly ISaveHeaterExecutor _saveHeaterExecutor;
        private readonly IRemoveHeaterExecutor _removeHeaterExecutor;

        public DeviceSetupController(IHeatingControl heatingControl,
                                     IBuildingDevicesProvider buildingDevicesProvider,
                                     IUpdateBuildingExecutor updateBuildingExecutor,
                                     IConnectedTemperatureSensorsProvider connectedTemperatureSensorsProvider,
                                     ISaveTemperatureSensorExecutor saveTemperatureSensorExecutor,
                                     IRemoveTemperatureSensorExecutor removeTemperatureSensorExecutor,
                                     II2c i2c,
                                     ISaveHeaterExecutor saveHeaterExecutor,
                                     IRemoveHeaterExecutor removeHeaterExecutor)
        {
            _heatingControl = heatingControl;
            _buildingDevicesProvider = buildingDevicesProvider;
            _updateBuildingExecutor = updateBuildingExecutor;
            _connectedTemperatureSensorsProvider = connectedTemperatureSensorsProvider;
            _saveTemperatureSensorExecutor = saveTemperatureSensorExecutor;
            _removeTemperatureSensorExecutor = removeTemperatureSensorExecutor;
            _i2C = i2c;
            _saveHeaterExecutor = saveHeaterExecutor;
            _removeHeaterExecutor = removeHeaterExecutor;
        }

        [HttpGet]
        public BuildingDevicesProviderResult GetDeviceConfiguration()
        {
            return _buildingDevicesProvider.Provide(_heatingControl.State, _heatingControl.Model);
        }

        [HttpPost("buildingName/{name}")]
        public void SetBuildingName(string name)
        {
            _updateBuildingExecutor.Execute(new UpdateBuildingExecutorInput
                                            {
                                                Name = name
                                            },
                                            _heatingControl.Model);
        }

        [HttpGet("connectedTemperatureSensors")]
        public ICollection<ConnectedTemperatureSensor> GetConnectedTemperetureSensors()
        {
            return _connectedTemperatureSensorsProvider.Provide(_heatingControl.Model);
        }

        [HttpPost("temperatureSensor")]
        public void AddTemperatureSensor([FromBody] SaveTemperatureSensorExecutorInput input)
        {
            _saveTemperatureSensorExecutor.Execute(input, _heatingControl.Model, _heatingControl.State);
        }

        [HttpDelete("temperatureSensor/{temperatureSensorId}")]
        public void RemoveTemperatureSensor(int temperatureSensorId)
        {
            _removeTemperatureSensorExecutor.Execute(temperatureSensorId, _heatingControl.State, _heatingControl.Model);
        }

        [HttpGet("connectedHeaterModules")]
        public async Task<IList<int>> GetAvailableHeaterModules()
        {
            return await _i2C.GetI2cDevices();
        }

        [HttpPost("heater")]
        public void AddHeater([FromBody] SaveHeaterExecutorInput input)
        {
            _saveHeaterExecutor.Execute(input, _heatingControl.State, _heatingControl.Model);
        }

        [HttpDelete("heater/{heaterId}")]
        public void RemoveHeater(int heaterId)
        {
            _removeHeaterExecutor.Execute(heaterId, _heatingControl.State, _heatingControl.Model);
        }
    }
}
