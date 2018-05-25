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

        public DeviceSetupController(IHeatingControl heatingControl,
                                     IBuildingDevicesProvider buildingDevicesProvider,
                                     IUpdateBuildingExecutor updateBuildingExecutor,
                                     IConnectedTemperatureSensorsProvider connectedTemperatureSensorsProvider,
                                     ISaveTemperatureSensorExecutor saveTemperatureSensorExecutor,
                                     IRemoveTemperatureSensorExecutor removeTemperatureSensorExecutor,
                                     II2c i2c)
        {
            _heatingControl = heatingControl;
            _buildingDevicesProvider = buildingDevicesProvider;
            _updateBuildingExecutor = updateBuildingExecutor;
            _connectedTemperatureSensorsProvider = connectedTemperatureSensorsProvider;
            _saveTemperatureSensorExecutor = saveTemperatureSensorExecutor;
            _removeTemperatureSensorExecutor = removeTemperatureSensorExecutor;
            _i2C = i2c;
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
        public void AddTemperatureSensor(SaveTemperatureSensorExecutorInput input)
        {
            _saveTemperatureSensorExecutor.Execute(input, _heatingControl.Model, _heatingControl.State);
        }

        [HttpDelete("temperatureSensor/{id}")]
        public void RemoveTemperatureSensor(int id)
        {
            _removeTemperatureSensorExecutor.Execute(id, _heatingControl.State, _heatingControl.Model);
        }

        [HttpGet("connectedHeaterModules")]
        public async Task<IList<int>> GetAvailableHeaterModules()
        {
            return await _i2C.GetI2cDevices();
        }

        [HttpPost("heater")]
        public void AddHeater(){}

        [HttpDelete("heater/{id}")]
        public void RemoveHeater(int id) {}

    }
}
