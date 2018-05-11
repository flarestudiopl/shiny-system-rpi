using Commons;
using Domain.BuildingModel;
using Microsoft.Extensions.Configuration;

namespace Storage.BuildingModel
{
    public interface IBuildingModelSaver
    {
        void Save(Building buildingModel);
    }

    public class BuildingModelSaver : IBuildingModelSaver
    {
        public const string BuildingModelConfigPath = "ConfigurationFiles:BuildingModel";

        private readonly IConfiguration _configuration;

        public BuildingModelSaver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Save(Building buildingModel)
        {
            var buildingModelFilePath = _configuration[BuildingModelConfigPath];

            JsonFile.Write(buildingModelFilePath, buildingModel);
        }
    }
}
