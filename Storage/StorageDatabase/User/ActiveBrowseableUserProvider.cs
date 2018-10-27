using Dapper;
using System.Linq;

namespace Storage.StorageDatabase.User
{
    public interface IActiveBrowseableUserProvider
    {
        Domain.StorageDatabase.User Provider(int id);
    }

    public class ActiveBrowseableUserProvider : IActiveBrowseableUserProvider
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public ActiveBrowseableUserProvider(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public Domain.StorageDatabase.User Provider(int UserId)
        {
            const string query = @"
SELECT *
FROM [User]
WHERE [UserId] = @UserId AND
      [IsActiveBool] = 1 AND
      [IsBrowseableBool] = 1";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                return connection.Query<Domain.StorageDatabase.User>(query,
                                                                    new
                                                                    {
                                                                        UserId = UserId
                                                                    })
                                 .SingleOrDefault();
            }
        }
    }
}
