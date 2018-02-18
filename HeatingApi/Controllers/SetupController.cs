using HeatingControl.Domain;
using Microsoft.AspNetCore.Mvc;
using Storage.BuildingModel;

namespace HeatingApi.Controllers
{
    [Produces("application/json")]
    [Route("/api/setup")]
    public class SetupController : Controller
    {
        private readonly IBuildingModelProvider _buildingModelProvider;
        private readonly IBuildingModelSaver _buildingModelSaver;

        public SetupController(IBuildingModelProvider buildingModelProvider,
                               IBuildingModelSaver buildingModelSaver)
        {
            _buildingModelProvider = buildingModelProvider;
            _buildingModelSaver = buildingModelSaver;
        }

        [HttpGet]
        public Building GetBuildingModel()
        {
            return _buildingModelProvider.Provide();
        }

        [HttpPut]
        public void SaveBuildingModel([FromBody] Building building)
        {
            // TODO validation
            _buildingModelSaver.Save(building);
        }
    }
}
