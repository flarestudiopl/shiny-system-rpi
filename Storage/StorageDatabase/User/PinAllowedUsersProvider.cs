using System.Collections.Generic;
using Dapper;

namespace Storage.StorageDatabase.User
{
    public interface IPinAllowedUsersProvider
    {
        ICollection<Domain.StorageDatabase.User> Provide();
    }

    public class PinAllowedUsersProvider : IPinAllowedUsersProvider
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public PinAllowedUsersProvider(ISqlConnectionResolver sqlConnectionResolver)
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
      [IsPinLoginAllowed] = 1";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                return connection.Query<Domain.StorageDatabase.User>(query)
                                 .AsList();
            }
        }
    }
}
