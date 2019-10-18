using Storage.StorageDatabase;

namespace HeatingControl.Application.DataAccess.Building
{
    public interface IBuildingNameUpdater
    {
        void Update(string name, Domain.Building building);
    }

    public class BuildingNameUpdater : IBuildingNameUpdater
    {
        private readonly IDbExecutor _dbExecutor;

        public BuildingNameUpdater(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public void Update(string name, Domain.Building building)
        {
            _dbExecutor.Execute(c =>
            {
                c.Attach(building);

                building.Name = name;

                c.SaveChanges();
            });
        }
    }
}
