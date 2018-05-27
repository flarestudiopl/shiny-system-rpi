using HeatingControl.Application.Commands;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Route("/api/setup/powerZone")]
    public class PowerZoneSetupController : BaseController
    {
        private readonly IHeatingControl _heatingControl;
        private readonly IPowerZoneListProvider _powerZoneListProvider;
        private readonly INewPowerZoneOptionsProvider _newPowerZoneOptionsProvider;
        private readonly IPowerZoneSettingsProvider _powerZoneSettingsProvider;
        private readonly ISavePowerZoneExecutor _savePowerZoneExecutor;
        private readonly IRemovePowerZoneExecutor _removePowerZoneExecutor;

        public PowerZoneSetupController(IHeatingControl heatingControl,
                                        IPowerZoneListProvider powerZoneListProvider,
                                        INewPowerZoneOptionsProvider newPowerZoneOptionsProvider,
                                        IPowerZoneSettingsProvider powerZoneSettingsProvider,
                                        ISavePowerZoneExecutor savePowerZoneExecutor,
                                        IRemovePowerZoneExecutor removePowerZoneExecutor)
        {
            _heatingControl = heatingControl;
            _powerZoneListProvider = powerZoneListProvider;
            _newPowerZoneOptionsProvider = newPowerZoneOptionsProvider;
            _powerZoneSettingsProvider = powerZoneSettingsProvider;
            _savePowerZoneExecutor = savePowerZoneExecutor;
            _removePowerZoneExecutor = removePowerZoneExecutor;
        }

        /// <summary>
        /// Provides list of power zones. To be used by power zone settings grid.
        /// </summary>
        [HttpGet]
        public PowerZoneListProviderResult GetPowerZoneList()
        {
            return _powerZoneListProvider.Provide(_heatingControl.Model, _heatingControl.State);
        }

        [HttpGet("new")]
        public NewPowerZoneOptionsProviderResult GetAvailableDevices()
        {
            return _newPowerZoneOptionsProvider.Provide(_heatingControl.State, _heatingControl.Model);
        }

        /// <summary>
        /// Provides data for power zone settings editor.
        /// </summary>
        [HttpGet("{powerZoneId}")]
        public PowerZoneSettingsProviderResult GetPowerZoneSettings(int powerZoneId)
        {
            return _powerZoneSettingsProvider.Provide(powerZoneId, _heatingControl.State);
        }

        /// <summary>
        /// Saves new or existing power zone. Power zone settings editor should post data here.
        /// </summary>
        [HttpPost]
        public void SavePowerZoneSettings([FromBody] SavePowerZoneExecutorInput input)
        {
            _savePowerZoneExecutor.Execute(input, _heatingControl.Model, _heatingControl.State);
        }

        /// <summary>
        /// Allows to remove power zone. To be used by power zone settings grid (or editor).
        /// </summary>
        [HttpDelete("{powerZoneId}")]
        public void RemovePowerZone(int powerZoneId)
        {
            _removePowerZoneExecutor.Execute(powerZoneId, _heatingControl.State, _heatingControl.Model);
        }
    }
}
