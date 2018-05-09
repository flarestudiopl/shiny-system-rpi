﻿using System;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using HeatingControl.Domain;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Produces("application/json")]
    [Route("/api/zone")]
    public class ZoneController : Controller
    {
        private readonly IHeatingControl _heatingControl;
        private readonly IZoneDetailsProvider _zoneDetailsProvider;
        private readonly ITemperatureSetPointExecutor _temperatureSetPointExecutor;
        private readonly INewScheduleItemExecutor _newScheduleItemExecutor;

        public ZoneController(IHeatingControl heatingControl,
                              IZoneDetailsProvider zoneDetailsProvider,
                              ITemperatureSetPointExecutor temperatureSetPointExecutor,
                              INewScheduleItemExecutor newScheduleItemExecutor)
        {
            _heatingControl = heatingControl;
            _zoneDetailsProvider = zoneDetailsProvider;
            _temperatureSetPointExecutor = temperatureSetPointExecutor;
            _newScheduleItemExecutor = newScheduleItemExecutor;
        }

        [HttpGet("{zoneId}")]
        public ZoneDetailsProviderResult GetDetails(int zoneId)
        {
            return _zoneDetailsProvider.Provide(zoneId, _heatingControl.State);
        }

        [HttpPost("{zoneId}/setMode/{controlMode}")]
        public void SetControlMode(int zoneId, ZoneControlMode controlMode)
        {
            // TODO - move to Commands
            _heatingControl.State.ZoneIdToState[zoneId].ControlMode = controlMode;
        }

        [HttpDelete("{zoneId}/resetCounters")]
        public void ResetCounters(int zoneId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{zoneId}/setLowSetPoint/{value}")]
        public void SetLowSetPoint(int zoneId, float value)
        {
            SetSetPoint(zoneId, value, SetPointType.Low);
        }

        [HttpPost("{zoneId}/setHighSetPoint/{value}")]
        public void SetHighSetPoint(int zoneId, float value)
        {
            SetSetPoint(zoneId, value, SetPointType.High);
        }

        [HttpPost("{zoneId}/setScheduleSetPoint/{value}")]
        public void SetScheduleSetPoint(int zoneId, float value)
        {
            SetSetPoint(zoneId, value, SetPointType.Schedule);
        }

        [HttpPost("{zoneId}/setHysteresisSetPoint/{value}")]
        public void SetHysteresisSetPoint(int zoneId, float value)
        {
            SetSetPoint(zoneId, value, SetPointType.Hysteresis);
        }

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
