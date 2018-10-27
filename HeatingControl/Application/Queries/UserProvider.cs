using Storage.StorageDatabase.User;

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
        private readonly IActiveBrowseableUserProvider _activeBrowseableUserProvider;

        public UserProvider(IActiveBrowseableUserProvider activeBrowseableUserProvider)
        {
            _activeBrowseableUserProvider = activeBrowseableUserProvider;
        }

        public UserProviderResult Provide(int userId)
        {
            var user = _activeBrowseableUserProvider.Provider(userId);

            return new UserProviderResult
            {
                Login = user?.Login
            };
        }
    }
}
