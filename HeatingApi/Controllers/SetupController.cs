using HeatingControl.Application.Queries;
using HeatingControl.Domain;
using Microsoft.AspNetCore.Mvc;
using Storage.BuildingModel;

namespace HeatingApi.Controllers
{
    [Produces("application/json")]
    [Route("/api/setup")]
    public class SetupController : Controller
    {
        private readonly IHeatingControl _heatingControl;
        private readonly IZoneSettingsProvider _zoneSettingsProvider;
        private readonly IBuildingModelProvider _buildingModelProvider;
        private readonly IBuildingModelSaver _buildingModelSaver;

        public SetupController(IHeatingControl heatingControl,
                               IZoneSettingsProvider zoneSettingsProvider,
                               IBuildingModelProvider buildingModelProvider,
                               IBuildingModelSaver buildingModelSaver)
        {
            _heatingControl = heatingControl;
            _zoneSettingsProvider = zoneSettingsProvider;
            _buildingModelProvider = buildingModelProvider;
            _buildingModelSaver = buildingModelSaver;
        }

        #region TODO - REMOVE

        [HttpGet]
        public Building GetBuildingModel()
        {
            return _buildingModelProvider.Provide();
        }

        [HttpPut]
        public void SaveBuildingModel([FromBody] Building building)
        {
            _buildingModelSaver.Save(building);
        }

        #endregion // TODO - REMOVE

        [HttpGet("zone/{zoneId}")]
        public ZoneSettingsProviderResult GetZoneSettings(int zoneId)
        {
            return _zoneSettingsProvider.Provide(zoneId, _heatingControl.State, _heatingControl.Model);
        }
    }
}
