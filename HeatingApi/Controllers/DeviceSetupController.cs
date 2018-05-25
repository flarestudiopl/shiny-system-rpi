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

        public DeviceSetupController(IHeatingControl heatingControl,
                                     IBuildingDevicesProvider buildingDevicesProvider,
                                     IUpdateBuildingExecutor updateBuildingExecutor)
        {
            _heatingControl = heatingControl;
            _buildingDevicesProvider = buildingDevicesProvider;
            _updateBuildingExecutor = updateBuildingExecutor;
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

        [HttpGet("temperatureSensor")]
        public void GetAvailableTemperatureSensors(){}

        [HttpPost("temperatureSensor")]
        public void AddTemperatureSensor(){}

        [HttpDelete("temperatureSensor/{id}")]
        public void RemoveRemperatureSensor(int id){}

        [HttpGet("heaterModule")]
        public void GetAvailableHeaterModules(){}

        [HttpPost("heater")]
        public void AddHeater(){}

        [HttpDelete("heater/{id}")]
        public void RemoveHeater(int id) {}

    }
}
