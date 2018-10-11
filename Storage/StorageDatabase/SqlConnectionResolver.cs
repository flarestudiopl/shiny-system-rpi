using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Storage.StorageDatabase
{
    public interface ISqlConnectionResolver
    {
        IDbConnection Resolve();
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

        public IDbConnection Resolve()
        {
            var connection = new SqliteConnection(_connectionString);

            connection.Open();

            return connection;
        }
    }
}
