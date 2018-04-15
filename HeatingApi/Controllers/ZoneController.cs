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

        [HttpGet("{name}")]
        public ZoneState GetState(string name)
        {
            // TODO wrap state, don't let HC.Models to show up here
            return _heatingControl.State.ZoneNameToState[name];
        }
    }
}
