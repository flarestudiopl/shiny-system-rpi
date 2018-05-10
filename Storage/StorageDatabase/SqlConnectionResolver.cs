using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Storage.StorageDatabase
{
    public interface ISqlConnectionResolver
    {
        DbConnection Resolve();
    }

    public class SqlConnectionResolver : ISqlConnectionResolver
    {
        private const string StorageDatabaseConfigPath = "ConfigurationFiles:StorageDatabase";

        private readonly string _connectionString;

        public SqlConnectionResolver(IConfiguration configuration)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
                                          {
                                              DataSource = configuration[StorageDatabaseConfigPath]
                                          };

            _connectionString = connectionStringBuilder.ConnectionString;
        }

        public DbConnection Resolve()
        {
            var connection = new SqliteConnection(_connectionString);

            connection.Open();

            return connection;
        }
    }
}
