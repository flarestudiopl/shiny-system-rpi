using System;
using System.Security.Cryptography.X509Certificates;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Domain.BuildingModel;
using Microsoft.AspNetCore.Mvc;
using Storage.BuildingModel;

namespace HeatingApi.Controllers
{
    /// <summary>
    /// Controller for settings views
    /// </summary>
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
        private readonly IUserListProvider _userListProvider;

        public SetupController(IHeatingControl heatingControl,
                               IZoneListProvider zoneListProvider,
                               IZoneSettingsProvider zoneSettingsProvider,
                               ISaveZoneExecutor saveZoneExecutor,
                               IBuildingModelProvider buildingModelProvider,
                               IBuildingModelSaver buildingModelSaver,
                               IUserListProvider userListProvider)
        {
            _heatingControl = heatingControl;
            _zoneListProvider = zoneListProvider;
            _zoneSettingsProvider = zoneSettingsProvider;
            _saveZoneExecutor = saveZoneExecutor;
            _buildingModelProvider = buildingModelProvider;
            _buildingModelSaver = buildingModelSaver;
            _userListProvider = userListProvider;
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

        /// <summary>
        /// Provides list of zones. To be used by zone settings grid.
        /// </summary>
        [HttpGet("zone")]
        public ZoneListProviderOutput GetZoneList()
        {
            return _zoneListProvider.Provide(_heatingControl.State, _heatingControl.Model);
        }

        /// <summary>
        /// Provides data for zone settings editor.
        /// </summary>
        [HttpGet("zone/{zoneId}")]
        public ZoneSettingsProviderResult GetZoneSettings(int zoneId)
        {
            return _zoneSettingsProvider.Provide(zoneId, _heatingControl.State, _heatingControl.Model);
        }

        /// <summary>
        /// Saves new or existing zone. Zone settings editor should post data here.
        /// </summary>
        [HttpPost("zone")]
        public void SaveZoneSettings([FromBody] SaveZoneExecutorInput input)
        {
            _saveZoneExecutor.Execute(input, _heatingControl.Model, _heatingControl.State);
        }

        /// <summary>
        /// Allows to remove zone. To be used by zone settings grid (or editor).
        /// </summary>
        [HttpDelete("zone/{zoneId}")]
        public void RemoveZone(int zoneId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("user")]
        public UserListProviderResult GetUserList()
        {
            return _userListProvider.Provide();
        }
    }
}
