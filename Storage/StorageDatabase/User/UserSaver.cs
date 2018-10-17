using System;
using Dapper;

namespace Storage.StorageDatabase.User
{
    public interface IUserSaver
    {
        void Save(string login, string passwordHash, string pinHash, int createdByUserId);
    }

    public class UserSaver : IUserSaver
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public UserSaver(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public void Save(string login, string passwordHash, string pinHash, int createdByUserId)
        {
            const string query = @"
INSERT INTO [User]
([Login], [PasswordHash], [CreatedDateTime], [CreatedBy], [LastLogonDateTime], [QuickLoginPinHash])
VALUES
(@Login, @PasswordHash, @CreatedDateTime, @CreatedBy, @LastLogonDateTime, @QuickLoginPinHash)";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                var now = DateTime.Now.Ticks;

                connection.Execute(query,
                                   new
                                   {
                                       Login = login,
                                       PasswordHash = passwordHash,
                                       CreatedDateTime = now,
                                       CreatedBy = createdByUserId,
                                       LastLogonDateTime = now,
                                       QuickLoginPinHash = pinHash
                                   });
            }
        }
    }
}
