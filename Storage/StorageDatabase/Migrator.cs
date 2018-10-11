using DbUp;
using DbUp.SQLite.Helpers;
using System.Reflection;

namespace Storage.StorageDatabase
{
    public interface IMigrator
    {
        void Run();
    }

    public class Migrator : IMigrator
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public Migrator(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public void Run()
        {
            using (var connection = _sqlConnectionResolver.Resolve())
            {
                var upgrader = DeployChanges.To
                                            .SQLiteDatabase(new SharedConnection(connection))
                                            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                                            .LogToConsole()
                                            .Build();

                var result = upgrader.PerformUpgrade();

                if (result.Error != null)
                {
                    throw result.Error;
                }
            }
        }
    }
}
