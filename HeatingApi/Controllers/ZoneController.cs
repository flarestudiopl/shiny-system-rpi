using HeatingControl;
using HeatingControl.Models;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Produces("application/json")]
    [Route("/api/zone")]
    public class ZoneController : Controller
    {
        private readonly IHeatingControl _heatingControl;

        public ZoneController(IHeatingControl heatingControl)
        {
            _heatingControl = heatingControl;
        }

        [HttpGet("{zoneId}")]
        public ZoneState GetState(int zoneId)
        {
            return _heatingControl.State.ZoneIdToState[zoneId];
        }
    }
}
