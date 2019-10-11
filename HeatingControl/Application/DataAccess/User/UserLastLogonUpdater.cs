using Storage.StorageDatabase;
using System;

namespace HeatingControl.Application.DataAccess.User
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
        private readonly IDbExecutor _dbExecutor;

        public UserLastLogonUpdater(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public void Update(UserLastLogonUpdaterInput input)
        {
            _dbExecutor.Execute(c =>
            {
                var user = c.Users.Find(input.UserId);

                if (user != null)
                {
                    user.LastLogonDate = input.LastLogonDate;
                    user.LastSeenIpAddress = input.LastSeenIpAddress;

                    c.SaveChanges();
                }
            });
        }
    }
}
