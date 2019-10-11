using Domain;
using HeatingControl.Application.DataAccess.User;
using System.Collections.Generic;
using System.Linq;

namespace HeatingControl.Application.Queries
{
    public interface IUserProvider
    {
        UserProviderResult Provide(int id);
    }

    public class UserProviderResult
    {
        public string Login { get; set; }
        public ICollection<Permission> Permissions { get; set; }
    }

    public class UserProvider : IUserProvider
    {
        private readonly IActiveUserProvider _activeUserProvider;

        public UserProvider(IActiveUserProvider activeUserProvider)
        {
            _activeUserProvider = activeUserProvider;
        }

        public UserProviderResult Provide(int userId)
        {
            var user = _activeUserProvider.Provide(x => x.IsBrowseable && x.UserId == userId);

            if (user == null)
            {
                return null;
            }

            return new UserProviderResult
            {
                Login = user.Login,
                Permissions = user.UserPermissions.Select(x => x.Permission).ToArray()
            };
        }
    }
}
