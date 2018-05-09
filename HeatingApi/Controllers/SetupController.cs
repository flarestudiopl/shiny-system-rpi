using System;
using HeatingControl.Application.Commands;
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
        private readonly IZoneListProvider _zoneListProvider;
        private readonly IZoneSettingsProvider _zoneSettingsProvider;
        private readonly ISaveZoneExecutor _saveZoneExecutor;
        private readonly IBuildingModelProvider _buildingModelProvider;
        private readonly IBuildingModelSaver _buildingModelSaver;

        public SetupController(IHeatingControl heatingControl,
                               IZoneListProvider zoneListProvider,
                               IZoneSettingsProvider zoneSettingsProvider,
                               ISaveZoneExecutor saveZoneExecutor,
                               IBuildingModelProvider buildingModelProvider,
                               IBuildingModelSaver buildingModelSaver)
        {
            _heatingControl = heatingControl;
            _zoneListProvider = zoneListProvider;
            _zoneSettingsProvider = zoneSettingsProvider;
            _saveZoneExecutor = saveZoneExecutor;
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

        [HttpGet("zone")]
        public ZoneListProviderOutput GetZoneList()
        {
            return _zoneListProvider.Provide(_heatingControl.State, _heatingControl.Model);
        }

        [HttpGet("zone/{zoneId}")]
        public ZoneSettingsProviderResult GetZoneSettings(int zoneId)
        {
            return _zoneSettingsProvider.Provide(zoneId, _heatingControl.State, _heatingControl.Model);
        }

        [HttpPost("zone")]
        public void SaveZoneSettings([FromBody] SaveZoneExecutorInput input)
        {
            _saveZoneExecutor.Execute(input, _heatingControl.Model, _heatingControl.State);
        }

        [HttpDelete("zone/{zoneId}")]
        public void RemoveZone(int zoneId)
        {
            throw new NotImplementedException();
        }
    }
}
