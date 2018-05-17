using Dapper;

namespace Storage.StorageDatabase.User
{
    public interface IActiveUserByLoginProvider
    {
        Domain.StorageDatabase.User Provide(string login);
    }

    public class ActiveUserByLoginProvider : IActiveUserByLoginProvider
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public ActiveUserByLoginProvider(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public Domain.StorageDatabase.User Provide(string login)
        {
            const string query = @"
SELECT *
FROM [User]
WHERE [Login] = @Login AND
      [IsActiveBool] = 1";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                return connection.QuerySingleOrDefault<Domain.StorageDatabase.User>(query,
                                                                                    new
                                                                                    {
                                                                                        Login = login
                                                                                    });
            }
        }
    }
}
