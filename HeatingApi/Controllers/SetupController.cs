using System;
using Domain.BuildingModel;
using Microsoft.AspNetCore.Mvc;
using Storage.BuildingModel;

namespace HeatingApi.Controllers
{
    [Obsolete]
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
    }
}
