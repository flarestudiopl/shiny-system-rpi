using System;
using System.Collections.Generic;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Route("/api/setup/powerZone")]
    public class PowerZoneSetupController : BaseController
    {
        private readonly IHeatingControl _heatingControl;
        private readonly IPowerZoneListProvider _powerZoneListProvider;
        private readonly IPowerZoneSettingsProvider _powerZoneSettingsProvider;

        public PowerZoneSetupController(IHeatingControl heatingControl,
                                        IPowerZoneListProvider powerZoneListProvider,
                                        IPowerZoneSettingsProvider powerZoneSettingsProvider)
        {
            _heatingControl = heatingControl;
            _powerZoneListProvider = powerZoneListProvider;
            _powerZoneSettingsProvider = powerZoneSettingsProvider;
        }

        /// <summary>
        /// Provides list of power zones. To be used by power zone settings grid.
        /// </summary>
        [HttpGet]
        public ICollection<PowerZoneListItem> GetPowerZoneList()
        {
            return _powerZoneListProvider.Provide(_heatingControl.Model, _heatingControl.State);
        }

        /// <summary>
        /// Provides data for power zone settings editor.
        /// </summary>
        [HttpGet("{zoneId}")]
        public PowerZoneSettingsProviderResult GetPowerZoneSettings(int zoneId)
        {
            return _powerZoneSettingsProvider.Provide(zoneId, _heatingControl.State);
        }

        /// <summary>
        /// Saves new or existing power zone. Power zone settings editor should post data here.
        /// </summary>
        [HttpPost]
        public void SavePowerZoneSettings()
        {
            // TODO
        }

        /// <summary>
        /// Allows to remove power zone. To be used by power zone settings grid (or editor).
        /// </summary>
        [HttpDelete("{zoneId}")]
        public void RemovePowerZone(int zoneId)
        {
            throw new NotImplementedException();
        }
    }
}
