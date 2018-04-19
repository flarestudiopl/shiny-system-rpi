using HeatingControl.Application;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Produces("application/json")]
    [Route("/api/dashboard")]
    public class DashboardController : Controller
    {
        private readonly IDashboardSnapshotProvider _dashboardSnapshotProvider;

        public DashboardController(IDashboardSnapshotProvider dashboardSnapshotProvider)
        {
            _dashboardSnapshotProvider = dashboardSnapshotProvider;
        }

        [HttpGet]
        public DashboardSnapshotProviderOutput GetSnapshot()
        {
            return _dashboardSnapshotProvider.Provide();
        }
    }
}
