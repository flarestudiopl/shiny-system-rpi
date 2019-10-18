using Storage.StorageDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HeatingControl.Application.DataAccess
{
    [Obsolete]
    public interface IRepository<TItem> where TItem : class
    {
        TItem Create(TItem item, Domain.Building building);
        TItem Read(int id);
        ICollection<TItem> Read(Expression<Func<TItem, bool>> filterPredicate);
        TItem ReadSingle(Expression<Func<TItem, bool>> filterPredicate);
        TItem ReadSingleOrDefault(Expression<Func<TItem, bool>> filterPredicate);
        void Update(TItem item, Domain.Building building);
        void Delete(TItem item, Domain.Building building);
    }

    [Obsolete]
    public class Repository<TItem> : IRepository<TItem> where TItem : class
    {
        private readonly IDbExecutor _dbExecutor;

        public Repository(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public TItem Create(TItem item, Domain.Building building)
        {
            _dbExecutor.Execute(c =>
            {
                if (building != null)
                {
                    c.Attach(building);
                }

                c.Set<TItem>().Add(item);
                c.SaveChanges();
            });

            return item;
        }

        public TItem Read(int id)
        {
            return _dbExecutor.Query(c => c.Set<TItem>().Find(id));
        }

        public ICollection<TItem> Read(Expression<Func<TItem, bool>> filterPredicate)
        {
            return _dbExecutor.Query(c => c.Set<TItem>().Where(filterPredicate).ToList());
        }

        public TItem ReadSingle(Expression<Func<TItem, bool>> filterPredicate)
        {
            return _dbExecutor.Query(c => c.Set<TItem>().Single(filterPredicate));
        }

        public TItem ReadSingleOrDefault(Expression<Func<TItem, bool>> filterPredicate)
        {
            return _dbExecutor.Query(c => c.Set<TItem>().SingleOrDefault(filterPredicate));
        }

        public void Update(TItem item, Domain.Building building)
        {
            _dbExecutor.Execute(c =>
            {
                if (building != null)
                {
                    c.Attach(building);
                }

                c.Set<TItem>().Update(item);
                c.SaveChanges();
            });
        }

        public void Delete(TItem item, Domain.Building building)
        {
            _dbExecutor.Execute(c =>
            {
                if (building != null)
                {
                    c.Attach(building);
                }

                c.Set<TItem>().Remove(item);
                c.SaveChanges();
            });
        }
    }
}
