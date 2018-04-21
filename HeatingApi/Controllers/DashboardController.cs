using HeatingControl.Application;
using HeatingControl.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
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

        [HttpGet]
        public DashboardSnapshotProviderOutput GetSnapshot()
        {
            return _dashboardSnapshotProvider.Provide(_heatingControl.Model, _heatingControl.State);
        }
    }
}
