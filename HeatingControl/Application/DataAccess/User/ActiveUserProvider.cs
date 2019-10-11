using Microsoft.EntityFrameworkCore;
using Storage.StorageDatabase;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace HeatingControl.Application.DataAccess.User
{
    public interface IActiveUserProvider
    {
        Domain.User Provide(Expression<Func<Domain.User, bool>> predicate);
    }

    public class ActiveUserProvider : IActiveUserProvider
    {
        private readonly IDbExecutor _dbExecutor;

        public ActiveUserProvider(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public Domain.User Provide(Expression<Func<Domain.User, bool>> predicate)
        {
            return _dbExecutor.Query(c =>
            {
                return c.Users
                        .Include(u => u.UserPermissions)
                        .Where(x => x.IsActive)
                        .Where(predicate)
                        .SingleOrDefault();
            });
        }
    }
}
