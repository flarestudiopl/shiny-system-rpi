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
        private readonly IConnectedTemperatureSensorsProvider _connectedTemperatureSensorsProvider;
        private readonly INewHeaterOptionsProvider _newHeaterOptionsProvider;
        private readonly ICommandHandler _commandHandler;

        public DeviceSetupController(IHeatingControl heatingControl,
                                     IBuildingDevicesProvider buildingDevicesProvider,
                                     IConnectedTemperatureSensorsProvider connectedTemperatureSensorsProvider,
                                     INewHeaterOptionsProvider newHeaterOptionsProvider,
                                     ICommandHandler commandHandler)
        {
            _heatingControl = heatingControl;
            _buildingDevicesProvider = buildingDevicesProvider;
            _connectedTemperatureSensorsProvider = connectedTemperatureSensorsProvider;
            _newHeaterOptionsProvider = newHeaterOptionsProvider;
            _commandHandler = commandHandler;
        }

        [HttpGet]
        public BuildingDevicesProviderResult GetDeviceConfiguration()
        {
            return _buildingDevicesProvider.Provide(_heatingControl.State, _heatingControl.State.Model);
        }

        [HttpPost("buildingName/{name?}")]
        public IActionResult SetBuildingName(string name)
        {
            return _commandHandler.ExecuteCommand(new UpdateBuildingCommand
                                                  {
                                                      Name = name
                                                  },
                                                  UserId);
        }

        [HttpPost("controlState/{state}")]
        public void SetControllerState(bool state)
        {
            _heatingControl.SetControlEnabled(state);
        }

        [HttpGet("connectedTemperatureSensors")]
        public ICollection<ConnectedTemperatureSensor> GetConnectedTemperetureSensors()
        {
            return _connectedTemperatureSensorsProvider.Provide(_heatingControl.State.Model);
        }

        [HttpPost("temperatureSensor")]
        public IActionResult AddTemperatureSensor([FromBody] SaveTemperatureSensorCommand command)
        {
            return _commandHandler.ExecuteCommand(command, UserId);
        }

        [HttpDelete("temperatureSensor/{temperatureSensorId}")]
        public IActionResult RemoveTemperatureSensor(int temperatureSensorId)
        {
            return _commandHandler.ExecuteCommand(new RemoveTemperatureSensorCommand
                                                  {
                                                      SensorId = temperatureSensorId
                                                  },
                                                  UserId);
        }

        [HttpGet("heater/new")]
        public NewHeaterOptionsProviderResult GetNewHeaterOptions()
        {
            return _newHeaterOptionsProvider.Provide();
        }

        [HttpPost("heater")]
        public IActionResult AddHeater([FromBody] SaveHeaterCommand command)
        {
            return _commandHandler.ExecuteCommand(command, UserId);
        }

        [HttpDelete("heater/{heaterId}")]
        public IActionResult RemoveHeater(int heaterId)
        {
            return _commandHandler.ExecuteCommand(new RemoveHeaterCommand
                                                  {
                                                      HeaterId = heaterId

                                                  },
                                                  UserId);
        }
    }
}
