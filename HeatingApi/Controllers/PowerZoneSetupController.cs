﻿using Domain;
using HeatingApi.Attributes;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Route("/api/setup/powerZone")]
    [RequiredPermission(Permission.Configuration_PowerZones)]
    public class PowerZoneSetupController : BaseController
    {
        private readonly IHeatingControl _heatingControl;
        private readonly IPowerZoneListProvider _powerZoneListProvider;
        private readonly INewPowerZoneOptionsProvider _newPowerZoneOptionsProvider;
        private readonly IPowerZoneSettingsProvider _powerZoneSettingsProvider;
        private readonly ICommandHandler _commandHandler;

        public PowerZoneSetupController(IHeatingControl heatingControl,
                                        IPowerZoneListProvider powerZoneListProvider,
                                        INewPowerZoneOptionsProvider newPowerZoneOptionsProvider,
                                        IPowerZoneSettingsProvider powerZoneSettingsProvider,
                                        ICommandHandler commandHandler)
        {
            _heatingControl = heatingControl;
            _powerZoneListProvider = powerZoneListProvider;
            _newPowerZoneOptionsProvider = newPowerZoneOptionsProvider;
            _powerZoneSettingsProvider = powerZoneSettingsProvider;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Provides list of power zones. To be used by power zone settings grid.
        /// </summary>
        [HttpGet]
        public PowerZoneListProviderResult GetPowerZoneList()
        {
            return _powerZoneListProvider.Provide(_heatingControl.State.Model, _heatingControl.State);
        }

        [HttpGet("new")]
        public NewPowerZoneOptionsProviderResult GetAvailableDevices()
        {
            return _newPowerZoneOptionsProvider.Provide(_heatingControl.State, _heatingControl.State.Model);
        }

        /// <summary>
        /// Provides data for power zone settings editor.
        /// </summary>
        [HttpGet("{powerZoneId}")]
        public PowerZoneSettingsProviderResult GetPowerZoneSettings(int powerZoneId)
        {
            return _powerZoneSettingsProvider.Provide(powerZoneId, _heatingControl.State);
        }

        /// <summary>
        /// Saves new or existing power zone. Power zone settings editor should post data here.
        /// </summary>
        [HttpPost]
        public IActionResult SavePowerZoneSettings([FromBody] SavePowerZoneCommand command)
        {
            return _commandHandler.ExecuteCommand(command, UserId);
        }

        /// <summary>
        /// Allows to remove power zone. To be used by power zone settings grid (or editor).
        /// </summary>
        [HttpDelete("{powerZoneId}")]
        public IActionResult RemovePowerZone(int powerZoneId)
        {
            return _commandHandler.ExecuteCommand(new RemovePowerZoneCommand
                                                  {
                                                      PowerZoneId = powerZoneId
                                                  },
                                                  UserId);
        }
    }
}
