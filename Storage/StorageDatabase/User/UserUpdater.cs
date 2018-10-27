using Dapper;

namespace Storage.StorageDatabase.User
{
    public interface IUserUpdater
    {
        void Update(UserUpdaterInput input);
    }

    public class UserUpdaterInput
    {
        public int UserId { get; set; }
        public string PasswordHash { get; set; }
        public string QuickLoginPinHash { get; set; }
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
            const string query = @"
UPDATE [User] 
SET [PasswordHash] = COALESCE(@PasswordHash, [PasswordHash]),
    [QuickLoginPinHash] = COALESCE(@QuickLoginPinHash, [QuickLoginPinHash]) 
WHERE [UserId] = @UserId";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                connection.Execute(query,
                                   new
                                   {
                                       UserId = input.UserId,
                                       PasswordHash = input.PasswordHash,
                                       QuickLoginPinHash = input.QuickLoginPinHash
                                   });
            }
        }
    }
}
