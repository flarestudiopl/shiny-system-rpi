using System;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Domain.BuildingModel;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    /// <summary>
    /// Controller for zone state manipulation.
    /// </summary>
    [Produces("application/json")]
    [Route("/api/zone")]
    public class ZoneController : Controller
    {
        private readonly IHeatingControl _heatingControl;
        private readonly IZoneDetailsProvider _zoneDetailsProvider;
        private readonly ICounterResetExecutor _counterResetExecutor;
        private readonly IZoneControlModeExecutor _zoneControlModeExecutor;
        private readonly ITemperatureSetPointExecutor _temperatureSetPointExecutor;
        private readonly INewScheduleItemExecutor _newScheduleItemExecutor;

        public ZoneController(IHeatingControl heatingControl,
                              IZoneDetailsProvider zoneDetailsProvider,
                              ICounterResetExecutor counterResetExecutor,
                              IZoneControlModeExecutor zoneControlModeExecutor,
                              ITemperatureSetPointExecutor temperatureSetPointExecutor,
                              INewScheduleItemExecutor newScheduleItemExecutor)
        {
            _heatingControl = heatingControl;
            _zoneDetailsProvider = zoneDetailsProvider;
            _counterResetExecutor = counterResetExecutor;
            _zoneControlModeExecutor = zoneControlModeExecutor;
            _temperatureSetPointExecutor = temperatureSetPointExecutor;
            _newScheduleItemExecutor = newScheduleItemExecutor;
        }

        /// <summary>
        /// Returns data about timers, setpoints and schedule. To be used by zone details.
        /// </summary>
        [HttpGet("{zoneId}")]
        public ZoneDetailsProviderResult GetDetails(int zoneId)
        {
            return _zoneDetailsProvider.Provide(zoneId, _heatingControl.State, _heatingControl.Model);
        }

        /// <summary>
        /// Switches zone control mode(0 - off/lo, 1 - on/high, 2 - schedule). To be used by zone tile on dashboard.
        /// </summary>
        [HttpPost("{zoneId}/setMode/{controlMode}")]
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
        [HttpDelete("{zoneId}/resetCounters")]
        public void ResetCounters(int zoneId)
        {
            _counterResetExecutor.Execute(zoneId, /* TODO */ -1, _heatingControl.State);
        }

        /// <summary>
        /// Allows to set zone low setpoint. To be used by zone temperature setpoints view.
        /// </summary>
        [HttpPost("{zoneId}/setLowSetPoint/{value}")]
        public void SetLowSetPoint(int zoneId, float value)
        {
            SetSetPoint(zoneId, value, SetPointType.Low);
        }

        /// <summary>
        /// Allows to set zone high setpoint. To be used by zone temperature setpoints view.
        /// </summary>
        [HttpPost("{zoneId}/setHighSetPoint/{value}")]
        public void SetHighSetPoint(int zoneId, float value)
        {
            SetSetPoint(zoneId, value, SetPointType.High);
        }

        /// <summary>
        /// Allows to set zone schedule default setpoint. To be used by zone temperature setpoints view.
        /// </summary>
        [HttpPost("{zoneId}/setScheduleSetPoint/{value}")]
        public void SetScheduleSetPoint(int zoneId, float value)
        {
            SetSetPoint(zoneId, value, SetPointType.Schedule);
        }

        /// <summary>
        /// Allows to set zone hysteresis. To be used by zone temperature setpoints view.
        /// </summary>
        [HttpPost("{zoneId}/setHysteresisSetPoint/{value}")]
        public void SetHysteresisSetPoint(int zoneId, float value)
        {
            SetSetPoint(zoneId, value, SetPointType.Hysteresis);
        }

        /// <summary>
        /// Adds new schedule item to zone. To be used by zone schedule editor.
        /// </summary>
        [HttpPost("schedule")]
        public void NewScheduleItem([FromBody] NewScheduleItemExecutorInput input)
        {
            _newScheduleItemExecutor.Execute(input, _heatingControl.Model);
        }

        private void SetSetPoint(int zoneId, float value, SetPointType setPointType)
        {
            var executorInput = new TemperatureSetPoinExecutorInput
                                {
                                    SetPointType = setPointType,
                                    ZoneId = zoneId,
                                    Value = value
                                };

            _temperatureSetPointExecutor.Execute(executorInput, _heatingControl.Model);
        }
    }
}
