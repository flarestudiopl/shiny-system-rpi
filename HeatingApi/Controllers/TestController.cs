using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using HardwareAccess;
using System.Threading.Tasks;

namespace HeatingApi.Controllers
{
    [Produces("application/json")]
    public class TestController : Controller
    {
        private readonly IOneWire _oneWire;
        private readonly ITemperatureSensor _temperatureSensor;
        private readonly II2c _i2C;

        public TestController(IOneWire oneWire, ITemperatureSensor temperatureSensor, II2c i2c)
        {
            _oneWire = oneWire;
            _temperatureSensor = temperatureSensor;
            _i2C = i2c;
        }

        [Route("/api/test/devices")]
        public IList<string> WireDevices()
        {
            return _oneWire.GetDevicesList();
        }

        [Route("/api/test/data/{deviceId}")]
        public async Task<string> WireData(string deviceId)
        {
            return await _oneWire.GetDeviceData(deviceId);
        }

        [Route("/api/test/temp/{deviceId}")]
        public async Task<TemperatureSensorData> Temperature(string deviceId)
        {
            return await _temperatureSensor.Read(deviceId);
        }

        [Route("/api/test/pcf/{value}")]
        public void SetPcf(byte value)
        {
            _i2C.WriteToDevice(0x38, value);
        }

        [Route("/api/test/i2c")]
        public string GetI2c(byte value)
        {
            return _i2C.GetI2cDetectResult();
        }
    }
}
