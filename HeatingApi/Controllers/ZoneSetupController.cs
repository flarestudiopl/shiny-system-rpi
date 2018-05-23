using System;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    /// <summary>
    /// Controller for settings views
    /// </summary>
    [Route("/api/setup/zone")]
    public class ZoneSetupController : BaseController
    {
        private readonly IHeatingControl _heatingControl;
        private readonly IZoneListProvider _zoneListProvider;
        private readonly IZoneSettingsProvider _zoneSettingsProvider;
        private readonly ISaveZoneExecutor _saveZoneExecutor;

        public ZoneSetupController(IHeatingControl heatingControl,
                                   IZoneListProvider zoneListProvider,
                                   IZoneSettingsProvider zoneSettingsProvider,
                                   ISaveZoneExecutor saveZoneExecutor)
        {
            _heatingControl = heatingControl;
            _zoneListProvider = zoneListProvider;
            _zoneSettingsProvider = zoneSettingsProvider;
            _saveZoneExecutor = saveZoneExecutor;
        }

        /// <summary>
        /// Provides list of zones. To be used by zone settings grid.
        /// </summary>
        [HttpGet]
        public ZoneListProviderOutput GetZoneList()
        {
            return _zoneListProvider.Provide(_heatingControl.State, _heatingControl.Model);
        }

        /// <summary>
        /// Provides data for zone settings editor.
        /// </summary>
        [HttpGet("{zoneId}")]
        public ZoneSettingsProviderResult GetZoneSettings(int zoneId)
        {
            return _zoneSettingsProvider.Provide(zoneId, _heatingControl.State, _heatingControl.Model);
        }

        /// <summary>
        /// Saves new or existing zone. Zone settings editor should post data here.
        /// </summary>
        [HttpPost]
        public void SaveZoneSettings([FromBody] SaveZoneExecutorInput input)
        {
            _saveZoneExecutor.Execute(input, _heatingControl.Model, _heatingControl.State);
        }

        /// <summary>
        /// Allows to remove zone. To be used by zone settings grid (or editor).
        /// </summary>
        [HttpDelete("{zoneId}")]
        public void RemoveZone(int zoneId)
        {
            throw new NotImplementedException();
        }
    }
}
