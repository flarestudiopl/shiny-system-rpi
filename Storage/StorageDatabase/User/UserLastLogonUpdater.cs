using Dapper;
using System;

namespace Storage.StorageDatabase.User
{
    public interface IUserLastLogonUpdater
    {
        void Update(UserLastLogonUpdaterInput input);
    }

    public class UserLastLogonUpdaterInput
    {
        public int UserId { get; set; }
        public DateTime LastLogonDate { get; set; }
        public string LastSeenIpAddress { get; set; }
    }

    public class UserLastLogonUpdater : IUserLastLogonUpdater
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public UserLastLogonUpdater(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public void Update(UserLastLogonUpdaterInput input)
        {
            const string query = @"
UPDATE [User] 
SET [LastLogonDateTime] = @LastLogonDateTime,
    [LastSeenIpAddress] = @LastSeenIpAddress
WHERE [UserId] = @UserId";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                connection.Execute(query,
                                   new
                                   {
                                       input.UserId,
                                       input.LastSeenIpAddress,
                                       LastLogonDateTime = input.LastLogonDate.Ticks
                                   });
            }
        }
    }
}
