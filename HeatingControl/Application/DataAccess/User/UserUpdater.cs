using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Domain;
using Microsoft.EntityFrameworkCore;
using Storage.StorageDatabase;

namespace HeatingControl.Application.DataAccess.User
{
    public interface IUserUpdater
    {
        Domain.User Update(UserUpdaterInput input);
    }

    public class UserUpdaterInput
    {
        public int UserId { get; set; }
        public string PasswordHash { get; set; }
        public string PinHash { get; set; }
        public ICollection<Permission> Permissions { get; set; }
    }

    public class UserUpdater : IUserUpdater
    {
        private readonly IDbExecutor _dbExecutor;

        public UserUpdater(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public Domain.User Update(UserUpdaterInput input)
        {
            return _dbExecutor.Query(c =>
            {
                var user = c.Users
                            .Include(u => u.UserPermissions)
                            .SingleOrDefault(u => u.UserId == input.UserId);

                if (user != null)
                {
                    user.PasswordHash = input.PasswordHash ?? user.PasswordHash;
                    user.QuickLoginPinHash = input.PinHash ?? user.QuickLoginPinHash;

                    var permissionsToRemove = new HashSet<Permission>(user.UserPermissions.Select(x => x.Permission));

                    foreach (var permission in input.Permissions)
                    {
                        if (permissionsToRemove.Contains(permission))
                        {
                            permissionsToRemove.Remove(permission);
                        }
                        else
                        {
                            user.UserPermissions.Add(new UserPermission { Permission = permission });
                        }
                    }

                    foreach (var permissionToRemove in permissionsToRemove)
                    {
                        user.UserPermissions.Remove(x => x.Permission == permissionToRemove);
                    }

                    c.SaveChanges();
                }

                return user;
            });
        }
    }
}
