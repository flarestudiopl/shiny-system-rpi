using Microsoft.EntityFrameworkCore;

namespace Storage.StorageDatabase
{
    public interface IMigrator
    {
        void Run();
    }

    public class Migrator : IMigrator
    {
        private readonly IDbExecutor _dbExecutor;

        public Migrator(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public void Run()
        {
            _dbExecutor.Execute(c => c.Database.Migrate());
        }
    }
}
