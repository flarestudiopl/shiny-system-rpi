﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HardwareAccess.Buses;
using HardwareAccess.Devices;
using HeatingControl;

namespace HeatingApi.Controllers
{
    [Produces("application/json")]
    [Route("/api/[controller]")]
    public class TestController : Controller
    {
        private readonly IOneWire _oneWire;
        private readonly ITemperatureSensor _temperatureSensor;
        private readonly II2c _i2C;
        private readonly IPowerOutput _powerOutput;
        private readonly IHeatingControl _heatingControl;

        public TestController(IOneWire oneWire, 
                              ITemperatureSensor temperatureSensor, 
                              II2c i2c,
                              IPowerOutput powerOutput,
                              IHeatingControl heatingControl)
        {
            _oneWire = oneWire;
            _temperatureSensor = temperatureSensor;
            _i2C = i2c;
            _powerOutput = powerOutput;
            _heatingControl = heatingControl;
        }

        [HttpGet("1w/devices")]
        public IList<string> WireDevices()
        {
            return _oneWire.GetDevicesList();
        }

        [HttpGet("1w/{deviceId}/raw")]
        public async Task<string> WireData(string deviceId)
        {
            return await _oneWire.GetDeviceData(deviceId);
        }

        [HttpGet("1w/{deviceId}/temp")]
        public async Task<TemperatureSensorData> Temperature(string deviceId)
        {
            return await _temperatureSensor.Read(deviceId);
        }

        [HttpGet("i2c/devices")]
        public async Task<IList<int>> GetI2c()
        {
            return await _i2C.GetI2cDevices();
        }

        [HttpPost("i2c")]
        public void SetPcf(byte device, byte value)
        {
            _i2C.WriteToDevice(device, value);
        }

        [HttpPost("i2c/power")]
        public void SetPowerOutput(int deviceId, int channel, bool state)
        {
            _powerOutput.SetState(deviceId, channel, state);
        }

        [HttpGet("control/start")]
        public void ControlStart()
        {
            _heatingControl.Start();
        }

        [HttpGet("control/temp/{deviceId}")]
        public ControllerState.TemperatureData ControlTemp(string deviceId)
        {
            _heatingControl.State.DeviceIdToTemperatureData.TryGetValue(deviceId, out ControllerState.TemperatureData tempData);

            return tempData;
        }
    }
}
