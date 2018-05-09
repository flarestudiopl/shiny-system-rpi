using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    /// <summary>
    /// Controller for dashboard.
    /// </summary>
    [Produces("application/json")]
    [Route("/api/dashboard")]
    public class DashboardController : Controller
    {
        private readonly IHeatingControl _heatingControl;
        private readonly IDashboardSnapshotProvider _dashboardSnapshotProvider;

        public DashboardController(IHeatingControl heatingControl,
                                   IDashboardSnapshotProvider dashboardSnapshotProvider)
        {
            _heatingControl = heatingControl;
            _dashboardSnapshotProvider = dashboardSnapshotProvider;
        }

        /// <summary>
        /// Snapshot of current controller state. Should be periodically pulled by main view.
        /// </summary>
        /// <returns>Building state and notifications</returns>
        [HttpGet]
        public DashboardSnapshotProviderOutput GetSnapshot()
        {
            return _dashboardSnapshotProvider.Provide(_heatingControl.Model, _heatingControl.State);
        }
    }
}
