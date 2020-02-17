using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HardwareAccess.Buses;
using HeatingControl.Models;
using HeatingControl.Application.DataAccess.Counter;
using HeatingApi.Attributes;
using Domain;
using HardwareAccess.Devices.TemperatureInputs;

namespace HeatingApi.Controllers
{
    [Produces("application/json")]
    [Route("/api/[controller]")]
    [RequiredPermission(Permission.Configuration_Devices)]
    public class TestController : Controller
    {
        private readonly IOneWire _oneWire;
        private readonly IDs1820 _ds1820;
        private readonly II2c _i2C;
        private readonly IHeatingControl _heatingControl;
        private readonly ICounterAccumulator _counterAccumulator;

        public TestController(IOneWire oneWire,
                              IDs1820 ds1820,
                              II2c i2c,
                              IHeatingControl heatingControl,
                              ICounterAccumulator counterAccumulator)
        {
            _oneWire = oneWire;
            _ds1820 = ds1820;
            _i2C = i2c;
            _heatingControl = heatingControl;
            _counterAccumulator = counterAccumulator;
        }

        /// <summary>
        /// Test comment
        /// </summary>
        /// <returns></returns>
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
            return await _ds1820.GetValue(new Ds1820.InputDescriptor { DeviceId = deviceId });
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

        [HttpGet("control/temp/{sensorId}")]
        public TemperatureSensorState ControlTemp(int sensorId)
        {
            _heatingControl.State.TemperatureSensorIdToState.TryGetValue(sensorId, out var tempData);

            return tempData;
        }

        [HttpPut("counter/accumulate")]
        public void AccumulateCounter(int heaterId, int value)
        {
            _counterAccumulator.Accumulate(new CounterAccumulatorInput
            {
                HeaterId = heaterId,
                SecondsToAccumulate = value
            });
        }
    }
}
