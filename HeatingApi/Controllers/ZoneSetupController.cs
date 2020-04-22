using Domain;
using HeatingApi.Attributes;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    /// <summary>
    /// Controller for settings views
    /// </summary>
    [Route("/api/setup/zone")]
    [RequiredPermission(Permission.Configuration_Zones)]
    public class ZoneSetupController : BaseController
    {
        private readonly IHeatingControl _heatingControl;
        private readonly IZoneListProvider _zoneListProvider;
        private readonly IAvailableDevicesProvider _availableDevicesProvider;
        private readonly IZoneSettingsProvider _zoneSettingsProvider;
        private readonly ICommandHandler _commandHandler;

        public ZoneSetupController(IHeatingControl heatingControl,
                                   IZoneListProvider zoneListProvider,
                                   IAvailableDevicesProvider availableDevicesProvider,
                                   IZoneSettingsProvider zoneSettingsProvider,
                                   ICommandHandler commandHandler)
        {
            _heatingControl = heatingControl;
            _zoneListProvider = zoneListProvider;
            _availableDevicesProvider = availableDevicesProvider;
            _zoneSettingsProvider = zoneSettingsProvider;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Provides list of zones. To be used by zone settings grid.
        /// </summary>
        [HttpGet]
        public ZoneListProviderResult GetZoneList()
        {
            return _zoneListProvider.Provide(_heatingControl.State, _heatingControl.State.Model);
        }

        [HttpGet("new")]
        public AvailableDevicesProviderResult GetAvailableDevices()
        {
            return _availableDevicesProvider.Provide(_heatingControl.State, _heatingControl.State.Model);
        }

        /// <summary>
        /// Provides data for zone settings editor.
        /// </summary>
        [HttpGet("{zoneId}")]
        public ZoneSettingsProviderResult GetZoneSettings(int zoneId)
        {
            return _zoneSettingsProvider.Provide(zoneId, _heatingControl.State, _heatingControl.State.Model);
        }

        /// <summary>
        /// Saves new or existing zone. Zone settings editor should post data here.
        /// </summary>
        [HttpPost]
        public IActionResult SaveZoneSettings([FromBody] SaveZoneCommand command)
        {
            return _commandHandler.ExecuteCommand(command, UserId);
        }

        /// <summary>
        /// Allows to remove zone. To be used by zone settings grid (or editor).
        /// </summary>
        [HttpDelete("{zoneId}")]
        public IActionResult RemoveZone(int zoneId)
        {
            return _commandHandler.ExecuteCommand(new RemoveZoneCommand
                                                  {
                                                      ZoneId = zoneId
                                                  },
                                                  UserId);
        }
    }
}
