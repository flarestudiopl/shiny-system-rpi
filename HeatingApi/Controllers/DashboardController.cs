using Autofac;
using Domain.BuildingModel;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
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
        private readonly ICounterResetExecutor _counterResetExecutor;
        private readonly IZoneControlModeExecutor _zoneControlModeExecutor;
        private readonly ITemperatureSetPointExecutor _temperatureSetPointExecutor;
        private readonly IRemoveScheduleItemExecutor _removeScheduleItemExecutor;
        private readonly ICommandHandler _commandHandler;

        public DashboardController(IHeatingControl heatingControl,
                                   IDashboardSnapshotProvider dashboardSnapshotProvider,
                                   IZoneDetailsProvider zoneDetailsProvider,
                                   ICounterResetExecutor counterResetExecutor,
                                   IZoneControlModeExecutor zoneControlModeExecutor,
                                   ITemperatureSetPointExecutor temperatureSetPointExecutor,
                                   IRemoveScheduleItemExecutor removeScheduleItemExecutor,
                                   ICommandHandler commandHandler)
        {
            _heatingControl = heatingControl;
            _dashboardSnapshotProvider = dashboardSnapshotProvider;
            _zoneDetailsProvider = zoneDetailsProvider;
            _counterResetExecutor = counterResetExecutor;
            _zoneControlModeExecutor = zoneControlModeExecutor;
            _temperatureSetPointExecutor = temperatureSetPointExecutor;
            _removeScheduleItemExecutor = removeScheduleItemExecutor;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Snapshot of current controller state. Should be periodically pulled by main view.
        /// </summary>
        /// <returns>Building state and notifications</returns>
        [HttpGet]
        public DashboardSnapshotProviderOutput GetSnapshot()
        {
            return _dashboardSnapshotProvider.Provide(_heatingControl.State.Model, _heatingControl.State, _heatingControl.ControlEnabled);
        }

        /// <summary>
        /// Returns data about timers, setpoints and schedule. To be used by zone details.
        /// </summary>
        [HttpGet("zone/{zoneId}")]
        public ZoneDetailsProviderResult GetDetails(int zoneId)
        {
            return _zoneDetailsProvider.Provide(zoneId, _heatingControl.State, _heatingControl.State.Model);
        }

        /// <summary>
        /// Switches zone control mode(0 - off/lo, 1 - on/high, 2 - schedule). To be used by zone tile on dashboard.
        /// </summary>
        [HttpPost("zone/{zoneId}/setMode/{controlMode}")]
        public void SetControlMode(int zoneId, ZoneControlMode controlMode)
        {
            var input = new ZoneControlModeExecutorInput
                        {
                            ZoneId = zoneId,
                            ControlMode = controlMode
                        };

            _zoneControlModeExecutor.Execute(input, _heatingControl.State);
        }

        /// <summary>
        /// Clears zone counters. To be used by zone counters view.
        /// </summary>
        [HttpDelete("zone/{zoneId}/resetCounters")]
        public void ResetCounters(int zoneId)
        {
            _counterResetExecutor.Execute(zoneId, UserId, _heatingControl.State);
        }

        /// <summary>
        /// Allows to set zone low setpoint. To be used by zone temperature setpoints view.
        /// </summary>
        [HttpPost("zone/{zoneId}/setLowSetPoint/{value}")]
        public void SetLowSetPoint(int zoneId, float value)
        {
            SetSetPoint(zoneId, value, SetPointType.Low);
        }

        /// <summary>
        /// Allows to set zone high setpoint. To be used by zone temperature setpoints view.
        /// </summary>
        [HttpPost("zone/{zoneId}/setHighSetPoint/{value}")]
        public void SetHighSetPoint(int zoneId, float value)
        {
            SetSetPoint(zoneId, value, SetPointType.High);
        }

        /// <summary>
        /// Allows to set zone schedule default setpoint. To be used by zone temperature setpoints view.
        /// </summary>
        [HttpPost("zone/{zoneId}/setScheduleSetPoint/{value}")]
        public void SetScheduleSetPoint(int zoneId, float value)
        {
            SetSetPoint(zoneId, value, SetPointType.Schedule);
        }

        /// <summary>
        /// Allows to set zone hysteresis. To be used by zone temperature setpoints view.
        /// </summary>
        [HttpPost("zone/{zoneId}/setHysteresisSetPoint/{value}")]
        public void SetHysteresisSetPoint(int zoneId, float value)
        {
            SetSetPoint(zoneId, value, SetPointType.Hysteresis);
        }

        /// <summary>
        /// Adds new schedule item to zone. To be used by zone schedule editor.
        /// </summary>
        [HttpPost("zone/schedule")]
        public IActionResult NewScheduleItem([FromBody] NewScheduleItemCommand input)
        {
            return _commandHandler.ExecuteCommand(input);
        }

        [HttpDelete("zone/{zoneId}/schedule/{scheduleItemId}")]
        public void RemoveScheduleItem(int zoneId, int scheduleItemId)
        {
            _removeScheduleItemExecutor.Execute(zoneId, scheduleItemId, _heatingControl.State, _heatingControl.State.Model);
        }

        [HttpPost("controllerState/{state}")]
        public void SetControllerState(bool state)
        {
            _heatingControl.ControlEnabled = state;
        }

        private void SetSetPoint(int zoneId, float value, SetPointType setPointType)
        {
            var executorInput = new TemperatureSetPoinExecutorInput
                                {
                                    SetPointType = setPointType,
                                    ZoneId = zoneId,
                                    Value = value
                                };

            _temperatureSetPointExecutor.Execute(executorInput, _heatingControl.State.Model);
        }
    }
}
