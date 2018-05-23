using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    /// <summary>
    /// Controller for settings views
    /// </summary>
    [Route("/api/setup/device")]
    public class DeviceSetupController : BaseController
    {
        private readonly IHeatingControl _heatingControl;

        public DeviceSetupController(IHeatingControl heatingControl)
        {
            _heatingControl = heatingControl;
        }
    }
}
