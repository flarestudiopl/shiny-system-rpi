using Domain;
using HeatingControl.Application.DataAccess;

namespace HeatingControl.Application.Queries
{
    public interface IUserProvider
    {
        UserProviderResult Provide(int id);
    }

    public class UserProviderResult
    {
        public string Login { get; set; }
    }

    public class UserProvider : IUserProvider
    {
        private readonly IRepository<User> _userRepository;

        public UserProvider(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public UserProviderResult Provide(int userId)
        {
            var user = _userRepository.ReadSingleOrDefault(x => x.UserId == userId &&
                                                                x.IsActive &&
                                                                x.IsBrowseable);

            return new UserProviderResult
            {
                Login = user?.Login
            };
        }
    }
}
