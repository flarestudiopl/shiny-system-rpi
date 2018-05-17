using System;
using Dapper;

namespace Storage.StorageDatabase.User
{
    public interface IUserDeactivator
    {
        void Deactivate(int userId, int disabledByUserId);
    }

    public class UserDeactivator : IUserDeactivator
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public UserDeactivator(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public void Deactivate(int userId, int disabledByUserId)
        {
            const string query = @"
UPDATE [User]
SET [IsActiveBool] = 0,
    [DisabledBy] = @DisabledBy,
    [DisabledDateTime] = @DisabledDateTime
WHERE
    [UserId] = @UserId AND
    [IsActiveBool] = 1 AND
    [IsBrowseableBool] = 1";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                connection.Execute(query,
                                   new
                                   {
                                       UserId = userId,
                                       DisabledBy = disabledByUserId,
                                       DisabledDateTime = DateTime.Now.Ticks
                                   });
            }
        }
    }
}
