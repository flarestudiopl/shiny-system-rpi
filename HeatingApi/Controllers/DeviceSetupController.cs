using System.Collections.Generic;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HeatingApi.Attributes;
using Domain;

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
        private readonly ITemperatureSensorSettingsProvider _temperatureSensorSettingsProvider;
        private readonly IHeaterSettingsProvider _heaterSettingsProvider;
        private readonly ICommandHandler _commandHandler;

        public DeviceSetupController(IHeatingControl heatingControl,
                                     IBuildingDevicesProvider buildingDevicesProvider,
                                     IConnectedTemperatureSensorsProvider connectedTemperatureSensorsProvider,
                                     INewHeaterOptionsProvider newHeaterOptionsProvider,
                                     ITemperatureSensorSettingsProvider temperatureSensorSettingsProvider,
                                     IHeaterSettingsProvider heaterSettingsProvider,
                                     ICommandHandler commandHandler)
        {
            _heatingControl = heatingControl;
            _buildingDevicesProvider = buildingDevicesProvider;
            _connectedTemperatureSensorsProvider = connectedTemperatureSensorsProvider;
            _newHeaterOptionsProvider = newHeaterOptionsProvider;
            _temperatureSensorSettingsProvider = temperatureSensorSettingsProvider;
            _heaterSettingsProvider = heaterSettingsProvider;
            _commandHandler = commandHandler;
        }

        [HttpGet]
        public BuildingDevicesProviderResult GetDeviceConfiguration()
        {
            return _buildingDevicesProvider.Provide(_heatingControl.State, _heatingControl.State.Model);
        }

        [HttpPost("buildingName/{name?}")]
        [RequiredPermission(Permission.Configuration_Devices)]
        public IActionResult SetBuildingName(string name)
        {
            return _commandHandler.ExecuteCommand(new UpdateBuildingCommand
                                                  {
                                                      Name = name
                                                  },
                                                  UserId);
        }

        [HttpPost("controlState/{state}")]
        [RequiredPermission(Permission.Configuration_PowerOptions)]
        public void SetControllerState(bool state)
        {
            _heatingControl.SetControlEnabled(state);
        }

        [HttpPost("powerOff")]
        [RequiredPermission(Permission.Configuration_PowerOptions)]
        public IActionResult PowerOff()
        {
            _heatingControl.SetControlEnabled(false);

            return _commandHandler.ExecuteCommand(new PowerOffCommand(), UserId);
        }

        [HttpGet("connectedTemperatureSensors")]
        [RequiredPermission(Permission.Configuration_Devices)]
        public ICollection<AvailableTemperatureInputProtocol> GetConnectedTemperetureSensors()
        {
            return _connectedTemperatureSensorsProvider.Provide(_heatingControl.State.Model);
        }

        [HttpGet("temperatureSensor/{temperatureSensorId}")]
        [RequiredPermission(Permission.Configuration_Devices)]
        public TemperatureSensorSettings GetTemperatureSensor(int temperatureSensorId)
        {
            return _temperatureSensorSettingsProvider.Provide(temperatureSensorId, _heatingControl.State.Model);
        }

        [HttpPost("temperatureSensor")]
        [RequiredPermission(Permission.Configuration_Devices)]
        public IActionResult AddTemperatureSensor([FromBody] SaveTemperatureSensorCommand command)
        {
            return _commandHandler.ExecuteCommand(command, UserId);
        }

        [HttpDelete("temperatureSensor/{temperatureSensorId}")]
        [RequiredPermission(Permission.Configuration_Devices)]
        public IActionResult RemoveTemperatureSensor(int temperatureSensorId)
        {
            return _commandHandler.ExecuteCommand(new RemoveTemperatureSensorCommand
                                                  {
                                                      SensorId = temperatureSensorId
                                                  },
                                                  UserId);
        }

        [HttpGet("heater/new")]
        [RequiredPermission(Permission.Configuration_Devices)]
        public NewHeaterOptionsProviderResult GetNewHeaterOptions()
        {
            return _newHeaterOptionsProvider.Provide();
        }

        [HttpGet("heater/{heaterId}")]
        [RequiredPermission(Permission.Configuration_Devices)]
        public HeaterSettings GetHeater(int heaterId)
        {
            return _heaterSettingsProvider.Provide(heaterId, _heatingControl.State);
        }

        [HttpPost("heater")]
        [RequiredPermission(Permission.Configuration_Devices)]
        public IActionResult AddHeater([FromBody] SaveHeaterCommand command)
        {
            return _commandHandler.ExecuteCommand(command, UserId);
        }

        [HttpDelete("heater/{heaterId}")]
        [RequiredPermission(Permission.Configuration_Devices)]
        public IActionResult RemoveHeater(int heaterId)
        {
            return _commandHandler.ExecuteCommand(new RemoveHeaterCommand
                                                  {
                                                      HeaterId = heaterId

                                                  },
                                                  UserId);
        }

        [HttpPut("dateTime")]
        [RequiredPermission(Permission.Configuration_Devices)]
        public IActionResult SetDateTime([FromBody] SetDateTimeCommand command)
        {
            return _commandHandler.ExecuteCommand(command, UserId);
        }
    }
}
