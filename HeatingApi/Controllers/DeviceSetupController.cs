using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    /// <summary>
    /// Controller for settings views
    /// </summary>
    [Produces("application/json")]
    [Route("/api/setup/device")]
    public class DeviceSetupController : Controller
    {
        private readonly IHeatingControl _heatingControl;

        public DeviceSetupController(IHeatingControl heatingControl)
        {
            _heatingControl = heatingControl;
        }
    }
}
