using Microsoft.EntityFrameworkCore;
using Storage.StorageDatabase;
using System.Linq;

namespace HeatingControl.Application.DataAccess.Building
{
    public interface IBuildingProvider
    {
        Domain.Building ProvideDefault();
    }

    public class BuildingProvider : IBuildingProvider
    {
        private readonly IDbExecutor _dbExecutor;

        public BuildingProvider(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public Domain.Building ProvideDefault()
        {
            return _dbExecutor.Query(c => c.Buildings
                                           .Include(b => b.DigitalInputs)
                                           .Include(b => b.Heaters)
                                             .ThenInclude(h => h.DigitalOutput)
                                           .Include(b => b.PowerZones)
                                           .Include(b => b.TemperatureSensors)
                                           .Include(b => b.Zones)
                                             .ThenInclude(z => z.TemperatureControlledZone)
                                           .Include(b => b.Zones)
                                             .ThenInclude(z => z.Schedule)
                                           .SingleOrDefault(x => x.IsDefault));
        }
    }
}
