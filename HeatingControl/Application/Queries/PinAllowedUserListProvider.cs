using Domain.StorageDatabase;
using HeatingControl.Application.DataAccess;
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
        private readonly IRepository<User> _userRepository;

        public PinAllowedUserListProvider(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public PinAllowedUserListProviderResult Provide()
        {
            var pinAllowedUsers = _userRepository.Read(x => x.IsActive &&
                                                            x.IsBrowseable &&
                                                            x.QuickLoginPinHash != null);

            return new PinAllowedUserListProviderResult
            {
                Logins = pinAllowedUsers.Select(x => x.Login)
                                        .ToList()
            };
        }
    }
}
