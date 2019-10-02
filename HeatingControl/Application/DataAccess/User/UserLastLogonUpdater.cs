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
        private readonly IRepository<Domain.User> _userRepository;

        public UserLastLogonUpdater(IRepository<Domain.User> userRepository)
        {
            _userRepository = userRepository;
        }

        public void Update(UserLastLogonUpdaterInput input)
        {
            var user = _userRepository.Read(input.UserId);

            user.LastLogonDate = input.LastLogonDate;
            user.LastSeenIpAddress = input.LastSeenIpAddress;

            _userRepository.Update(user);
        }
    }
}
