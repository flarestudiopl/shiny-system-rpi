using Dapper;
using System;

namespace Storage.StorageDatabase.User
{
    public interface IUserUpdater
    {
        void Update(UserUpdaterInput input);
    }

    public class UserUpdaterInput
    {
        public int UserId { get; set; }
        public DateTime LastLogonDate { get; set; }
        public string LastSeenIpAddress { get; set; }
    }

    public class UserUpdater : IUserUpdater
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public UserUpdater(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public void Update(UserUpdaterInput input)
        {
            var query = @"
UPDATE [User] 
SET [LastLogonDateTime] = @LastLogonDateTime,
    [LastSeenIpAddress] = @LastSeenIpAddress
WHERE [UserId] = @UserId";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                connection.Execute(query,
                                   new
                                   {
                                       UserId = input.UserId,
                                       LastSeenIpAddress = input.LastSeenIpAddress,
                                       LastLogonDateTime = input.LastLogonDate.Ticks
                                   });
            }
        }
    }
}
