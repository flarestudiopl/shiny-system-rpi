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

        public ZoneController(IHeatingControl heatingControl,
                              IZoneDetailsProvider zoneDetailsProvider)
        {
            _heatingControl = heatingControl;
            _zoneDetailsProvider = zoneDetailsProvider;
        }

        [HttpGet("{zoneId}")]
        public ZoneDetailsProviderResult GetDetails(int zoneId)
        {
            return _zoneDetailsProvider.Provide(zoneId, _heatingControl.State, _heatingControl.Model);
        }

        [HttpPost("{zoneId}/{controlMode}")]
        public void SetControlMode(int zoneId, ZoneControlMode controlMode)
        {
            _heatingControl.State.ZoneIdToState[zoneId].ControlMode = controlMode;
        }
    }
}
