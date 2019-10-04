using Storage.StorageDatabase;

namespace HeatingControl.Application.DataAccess.Heater
{
    public interface IHeaterRemover
    {
        void Remove(Domain.Heater heater, Domain.Building building);
    }

    public class HeaterRemover : IHeaterRemover
    {
        private readonly IDbExecutor _dbExecutor;

        public HeaterRemover(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public void Remove(Domain.Heater heater, Domain.Building building)
        {
            _dbExecutor.Execute(c =>
            {
                c.Attach(building);

                building.Heaters.Remove(heater);

                c.Heaters.Remove(heater);
                c.DigitalOutputs.Remove(heater.DigitalOutput);
                
                c.SaveChanges();
            });
        }
    }
}
