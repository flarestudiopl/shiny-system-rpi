using Domain;
using Storage.StorageDatabase;

namespace HeatingControl.Application.DataAccess.Zone
{
    public interface IZoneRemover
    {
        void Remove(Domain.Zone zone, Domain.Building building);
    }

    public class ZoneRemover : IZoneRemover
    {
        private readonly IDbExecutor _dbExecutor;

        public ZoneRemover(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public void Remove(Domain.Zone zone, Domain.Building building)
        {
            _dbExecutor.Execute(c =>
            {
                c.Attach(building);

                building.Zones.Remove(zone);

                c.Zones.Remove(zone);

                if (zone.TemperatureControlledZone != null)
                {
                    c.Set<Domain.TemperatureControlledZone>().Remove(zone.TemperatureControlledZone);
                }

                if (zone.Schedule.Count > 0)
                {
                    c.Set<ScheduleItem>().RemoveRange(zone.Schedule);
                }

                c.SaveChanges();
            });
        }
    }
}
