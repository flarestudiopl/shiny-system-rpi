using System.Collections.Generic;
using System.Threading.Tasks;
using HardwareAccess.Buses;
using HardwareAccess.Devices;
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
        private readonly ITemperatureSensor _temperatureSensor;
        private readonly II2c _i2C;
        private readonly IPowerOutput _powerOutput;

        public DeviceSetupController(IHeatingControl heatingControl,
                                     IBuildingDevicesProvider buildingDevicesProvider,
                                     IUpdateBuildingExecutor updateBuildingExecutor,
                                     ITemperatureSensor temperatureSensor,
                                     II2c i2c)
        {
            _heatingControl = heatingControl;
            _buildingDevicesProvider = buildingDevicesProvider;
            _updateBuildingExecutor = updateBuildingExecutor;
            _temperatureSensor = temperatureSensor;
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
        public ICollection<string> GetConnectedTemperetureSensors()
        {
            return _temperatureSensor.GetAvailableSensors();
        }

        [HttpPost("temperatureSensor")]
        public void AddTemperatureSensor(){}

        [HttpDelete("temperatureSensor/{id}")]
        public void RemoveTemperatureSensor(int id){}

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
