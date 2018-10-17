using Storage.StorageDatabase.User;
using System.Collections.Generic;
using System.Linq;

namespace HeatingControl.Application.Queries
{
    public interface IPinAllowedUserListProvider
    {
        PinAllowedUserListProviderResult Provide();
    }

    public class PinAllowedUserListProviderResult
    {
        public IList<string> Logins { get; set; }
    }

    public class PinAllowedUserListProvider : IPinAllowedUserListProvider
    {
        private readonly IUsersWithPinProvider _usersWithPinProvider;

        public PinAllowedUserListProvider(IUsersWithPinProvider usersWithPinProvider)
        {
            _usersWithPinProvider = usersWithPinProvider;
        }

        public PinAllowedUserListProviderResult Provide()
        {
            return new PinAllowedUserListProviderResult
            {
                Logins = _usersWithPinProvider.Provide()
                                              .Select(x => x.Login)
                                              .ToList()
            };
        }
    }
}
