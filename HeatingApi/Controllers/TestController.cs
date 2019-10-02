using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HardwareAccess.Buses;
using HardwareAccess.Devices;
using HeatingControl.Models;
using HeatingControl.Application.DataAccess.Counter;

namespace HeatingApi.Controllers
{
    [Produces("application/json")]
    [Route("/api/[controller]")]
    public class TestController : Controller
    {
        private readonly IOneWire _oneWire;
        private readonly ITemperatureSensor _temperatureSensor;
        private readonly II2c _i2C;
        private readonly IPowerOutputProvider _powerOutputProvider;
        private readonly IHeatingControl _heatingControl;
        private readonly ICounterAccumulator _counterAccumulator;

        public TestController(IOneWire oneWire, 
                              ITemperatureSensor temperatureSensor, 
                              II2c i2c,
                              IPowerOutputProvider powerOutputProvider,
                              IHeatingControl heatingControl,
                              ICounterAccumulator counterAccumulator)
        {
            _oneWire = oneWire;
            _temperatureSensor = temperatureSensor;
            _i2C = i2c;
            _powerOutputProvider = powerOutputProvider;
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
        public void SetPowerOutput(string protocolName, int deviceId, string channel, bool state)
        {
            _powerOutputProvider.Provide(protocolName).SetState(deviceId, channel, state);
        }

        [HttpGet("control/temp/{deviceId}")]
        public TemperatureData ControlTemp(string deviceId)
        {
            _heatingControl.State.TemperatureDeviceIdToTemperatureData.TryGetValue(deviceId, out TemperatureData tempData);

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
