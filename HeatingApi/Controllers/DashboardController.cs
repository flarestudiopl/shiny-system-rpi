﻿using Domain;
using HeatingApi.Attributes;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using HeatingControl.Models;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    /// <summary>
    /// Controller for dashboard.
    /// </summary>
    [Route("/api/dashboard")]
    public class DashboardController : BaseController
    {
        private readonly IHeatingControl _heatingControl;
        private readonly IDashboardSnapshotProvider _dashboardSnapshotProvider;
        private readonly IZoneDetailsProvider _zoneDetailsProvider;
        private readonly ICommandHandler _commandHandler;

        public DashboardController(IHeatingControl heatingControl,
                                   IDashboardSnapshotProvider dashboardSnapshotProvider,
                                   IZoneDetailsProvider zoneDetailsProvider,
                                   ICommandHandler commandHandler)
        {
            _heatingControl = heatingControl;
            _dashboardSnapshotProvider = dashboardSnapshotProvider;
            _zoneDetailsProvider = zoneDetailsProvider;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Snapshot of current controller state. Should be periodically pulled by main view.
        /// </summary>
        /// <returns>Building state and notifications</returns>
        [HttpGet]
        [RequiredPermission(Permission.Dashboard)]
        public DashboardSnapshotProviderOutput GetSnapshot()
        {
            return _dashboardSnapshotProvider.Provide(_heatingControl.State.Model, _heatingControl.State);
        }

        /// <summary>
        /// Returns data about timers, setpoints and schedule. To be used by zone details.
        /// </summary>
        [HttpGet("zone/{zoneId}")]
        [RequiredPermission(Permission.Dashboard_ZoneSettings)]
        public ZoneDetailsProviderResult GetDetails(int zoneId)
        {
            return _zoneDetailsProvider.Provide(zoneId, _heatingControl.State, _heatingControl.State.Model);
        }

        /// <summary>
        /// Switches zone control mode(0 - off/lo, 1 - on/high, 2 - schedule). To be used by zone tile on dashboard.
        /// </summary>
        [HttpPost("zone/{zoneId}/setMode/{controlMode}")]
        [RequiredPermission(Permission.Dashboard)]
        public IActionResult SetControlMode(int zoneId, ZoneControlMode controlMode)
        {
            var command = new SetZoneControlModeCommand
                          {
                              ZoneId = zoneId,
                              ControlMode = controlMode
                          };

            return _commandHandler.ExecuteCommand(command, UserId);
        }

        /// <summary>
        /// Clears zone counters. To be used by zone counters view.
        /// </summary>
        [HttpDelete("zone/{zoneId}/resetCounters")]
        [RequiredPermission(Permission.Dashboard_ZoneSettings)]
        public IActionResult ResetCounters(int zoneId)
        {
            var command = new ResetCounterCommand
                          {
                              ZoneId = zoneId,
                              UserId = UserId
                          };

            return _commandHandler.ExecuteCommand(command, UserId);
        }

        /// <summary>
        /// Allows to set zone low setpoint. To be used by zone temperature setpoints view.
        /// </summary>
        [HttpPost("zone/{zoneId}/setLowSetPoint/{value}")]
        [RequiredPermission(Permission.Dashboard_ZoneSettings)]
        public IActionResult SetLowSetPoint(int zoneId, float value)
        {
            return SetSetPoint(zoneId, value, SetPointType.Low);
        }

        /// <summary>
        /// Allows to set zone high setpoint. To be used by zone temperature setpoints view.
        /// </summary>
        [HttpPost("zone/{zoneId}/setHighSetPoint/{value}")]
        [RequiredPermission(Permission.Dashboard_ZoneSettings)]
        public IActionResult SetHighSetPoint(int zoneId, float value)
        {
            return SetSetPoint(zoneId, value, SetPointType.High);
        }

        /// <summary>
        /// Allows to set zone schedule default setpoint. To be used by zone temperature setpoints view.
        /// </summary>
        [HttpPost("zone/{zoneId}/setScheduleSetPoint/{value}")]
        [RequiredPermission(Permission.Dashboard_ZoneSettings)]
        public IActionResult SetScheduleSetPoint(int zoneId, float value)
        {
            return SetSetPoint(zoneId, value, SetPointType.Schedule);
        }

        /// <summary>
        /// Allows to set zone hysteresis. To be used by zone temperature setpoints view.
        /// </summary>
        [HttpPost("zone/{zoneId}/setHysteresisSetPoint/{value}")]
        [RequiredPermission(Permission.Dashboard_ZoneSettings)]
        public IActionResult SetHysteresisSetPoint(int zoneId, float value)
        {
            return SetSetPoint(zoneId, value, SetPointType.Hysteresis);
        }

        /// <summary>
        /// Adds new schedule item to zone. To be used by zone schedule editor.
        /// </summary>
        [HttpPost("zone/schedule")]
        [RequiredPermission(Permission.Dashboard_ZoneSettings)]
        public IActionResult NewScheduleItem([FromBody] NewScheduleItemCommand command)
        {
            return _commandHandler.ExecuteCommand(command, UserId);
        }

        [HttpDelete("zone/{zoneId}/schedule/{scheduleItemId}")]
        [RequiredPermission(Permission.Dashboard_ZoneSettings)]
        public IActionResult RemoveScheduleItem(int zoneId, int scheduleItemId)
        {
            var command = new RemoveScheduleItemCommand
                          {
                              ZoneId = zoneId,
                              ScheduleItemId = scheduleItemId
                          };

            return _commandHandler.ExecuteCommand(command, UserId);
        }

        private IActionResult SetSetPoint(int zoneId, float value, SetPointType setPointType)
        {
            var command = new SetTemperatureCommand
                          {
                              SetPointType = setPointType,
                              ZoneId = zoneId,
                              Value = value
                          };

            return _commandHandler.ExecuteCommand(command, UserId);
        }
    }
}
