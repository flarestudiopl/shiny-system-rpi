using System;
using System.Collections.Generic;
using System.Linq;
using Storage.StorageDatabase.User;

namespace HeatingControl.Application.Queries
{
    public interface IUserListProvider
    {
        UserListProviderResult Provide();
    }

    public class UserListProviderResult
    {
        public ICollection<UserListItem> Users { get; set; }

        public class UserListItem
        {
            public int Id { get; set; }
            public string Login { get; set; }
            public DateTime? LastLogonDate { get; set; }
        }
    }

    public class UserListProvider : IUserListProvider
    {
        private readonly IActiveBrowseableUsersProvider _activeBrowseableUsersProvider;

        public UserListProvider(IActiveBrowseableUsersProvider activeBrowseableUsersProvider)
        {
            _activeBrowseableUsersProvider = activeBrowseableUsersProvider;
        }

        public UserListProviderResult Provide()
        {
            var users = _activeBrowseableUsersProvider.Provider()
                                                      .Select(x => new UserListProviderResult.UserListItem
                                                                   {
                                                                       Id = x.UserId,
                                                                       Login = x.Login,
                                                                       LastLogonDate = x.LastLogon
                                                                   })
                                                      .ToList();

            return new UserListProviderResult
                   {
                       Users = users
                   };
        }
    }
}
