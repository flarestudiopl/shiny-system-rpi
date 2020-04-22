using Microsoft.Extensions.Configuration;
using System;

namespace Storage.StorageDatabase
{
    public interface IDbExecutor
    {
        TResult Query<TResult>(Func<EfContext, TResult> contextFunc);
        void Execute(Action<EfContext> contextFunc);
    }

    public class DbExecutor : IDbExecutor
    {
        private readonly string _connectionString;
        private readonly bool _allowDatabaseSave;

        private const string StorageDatabaseConfigPath = "ConfigurationFiles:StorageDatabase";
        private const string AllowDatabaseSaveConfigPath = "ConfigurationFiles:AllowDatabaseSave";

        public DbExecutor(IConfiguration configuration)
        {
            _connectionString = $"Filename={configuration[StorageDatabaseConfigPath]}";
            _allowDatabaseSave = configuration.GetValue<bool>(AllowDatabaseSaveConfigPath);
        }

        public TResult Query<TResult>(Func<EfContext, TResult> contextFunc)
        {
            using (var context = new EfContext(_connectionString, _allowDatabaseSave))
            {
                return contextFunc(context);
            }
        }

        public void Execute(Action<EfContext> contextFunc)
        {
            using (var context = new EfContext(_connectionString, _allowDatabaseSave))
            {
                contextFunc(context);
            }
        }
    }
}
