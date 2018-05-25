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

        public DeviceSetupController(IHeatingControl heatingControl)
        {
            _heatingControl = heatingControl;
        }

        [HttpPost("buildingName/{name}")]
        public void SetBuildingName(string name)
        {
        }

        [HttpGet]
        public void GetDevices()
        {
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
