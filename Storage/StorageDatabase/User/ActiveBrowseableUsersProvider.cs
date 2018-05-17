using System.Collections.Generic;
using Dapper;

namespace Storage.StorageDatabase.User
{
    public interface IActiveBrowseableUsersProvider
    {
        ICollection<Domain.StorageDatabase.User> Provider();
    }

    public class ActiveBrowseableUsersProvider : IActiveBrowseableUsersProvider
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public ActiveBrowseableUsersProvider(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public ICollection<Domain.StorageDatabase.User> Provider()
        {
            const string query = @"
SELECT * 
FROM [User] 
WHERE [IsActiveBool] = 1 AND 
      [IsBrowseableBool] = 1";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                return connection.Query<Domain.StorageDatabase.User>(query)
                                 .AsList();
            }
        }
    }
}
