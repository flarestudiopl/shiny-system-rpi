using System.Collections.Generic;
using Dapper;

namespace Storage.StorageDatabase.User
{
    public interface IUsersWithPinProvider
    {
        ICollection<Domain.StorageDatabase.User> Provide();
    }

    public class UsersWithPinProvider : IUsersWithPinProvider
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public UsersWithPinProvider(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public ICollection<Domain.StorageDatabase.User> Provide()
        {
             const string query = @"
SELECT * 
FROM [User] 
WHERE [IsActiveBool] = 1 AND 
      [IsBrowseableBool] = 1 AND
      [QuickLoginPinHash] IS NOT NULL";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                return connection.Query<Domain.StorageDatabase.User>(query)
                                 .AsList();
            }
        }
    }
}
