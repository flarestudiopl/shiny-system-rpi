using System;
using Dapper;

namespace Storage.StorageDatabase.User
{
    public interface IUserSaver
    {
        void Save(string login, string passwordHash, int createdByUserId);
    }

    public class UserSaver : IUserSaver
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public UserSaver(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public void Save(string login, string passwordHash, int createdByUserId)
        {
            const string query = @"
INSERT INTO [User]
([Login], [PasswordHash], [CreatedDateTime], [CreatedBy])
VALUES
(@Login, @PasswordHash, @CreatedDateTime, @CreatedBy)";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                connection.Execute(query,
                                   new
                                   {
                                       Login = login,
                                       PasswordHash = passwordHash,
                                       CreatedDateTime = DateTime.Now.Ticks,
                                       CreatedBy = createdByUserId
                                   });
            }
        }
    }
}
