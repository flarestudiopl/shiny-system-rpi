using System;
using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Route("/api/setup/powerZone")]
    public class PowerZoneSetupController : BaseController
    {
        public PowerZoneSetupController()
        {
        }

        /// <summary>
        /// Provides list of power zones. To be used by power zone settings grid.
        /// </summary>
        [HttpGet]
        public void GetPowerZoneList()
        {
        }

        /// <summary>
        /// Provides data for power zone settings editor.
        /// </summary>
        [HttpGet("{zoneId}")]
        public void GetPowerZoneSettings(int zoneId)
        {
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
