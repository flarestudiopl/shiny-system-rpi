using System;
using System.Collections.Generic;
using System.Linq;
using Domain.StorageDatabase;
using HeatingControl.Application.DataAccess;

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
        private readonly IRepository<User> _userRepository;

        public UserListProvider(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public UserListProviderResult Provide()
        {
            var users = _userRepository.Read(x => x.IsActive &&
                                                  x.IsBrowseable)
                                       .Select(x => new UserListProviderResult.UserListItem
                                       {
                                           Id = x.UserId,
                                           Login = x.Login,
                                           LastLogonDate = x.LastLogonDate
                                       })
                                       .OrderBy(x => x.Login)
                                       .ToList();

            return new UserListProviderResult
            {
                Users = users
            };
        }
    }
}
