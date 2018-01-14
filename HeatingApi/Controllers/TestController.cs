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

        public TestController(IOneWire oneWire, ITemperatureSensor temperatureSensor)
        {
            _oneWire = oneWire;
            _temperatureSensor = temperatureSensor;
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
    }
}
