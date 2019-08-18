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
        private const string StorageDatabaseConfigPath = "ConfigurationFiles:StorageDatabase";

        public DbExecutor(IConfiguration configuration)
        {
            _connectionString = $"Filename={configuration[StorageDatabaseConfigPath]}";
        }

        public TResult Query<TResult>(Func<EfContext, TResult> contextFunc)
        {
            using (var context = new EfContext(_connectionString))
            {
                return contextFunc(context);
            }
        }

        public void Execute(Action<EfContext> contextFunc)
        {
            using (var context = new EfContext(_connectionString))
            {
                contextFunc(context);
            }
        }
    }
}
