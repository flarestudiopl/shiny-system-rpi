using System.Collections.Generic;
using Dapper;

namespace Storage.StorageDatabase.Counter
{
    public interface ICurrentCountersByHeaterIdsProvider
    {
        ICollection<Domain.StorageDatabase.Counter> Provide(ICollection<int> heaterIds);
    }

    public class CurrentCountersByHeaterIdsProvider : ICurrentCountersByHeaterIdsProvider
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public CurrentCountersByHeaterIdsProvider(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public ICollection<Domain.StorageDatabase.Counter> Provide(ICollection<int> heaterIds)
        {
            const string sql = @"
SELECT *
FROM [Counter]
WHERE [HeaterId] IN @HeaterIds AND
      [ResetDateTime] IS NULL";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                return connection.Query<Domain.StorageDatabase.Counter>(sql,
                                                                        new
                                                                        {
                                                                            HeaterIds = heaterIds
                                                                        })
                                 .AsList();
            }
        }
    }
}
