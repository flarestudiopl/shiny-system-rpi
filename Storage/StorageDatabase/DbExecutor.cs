using System;

namespace Storage.StorageDatabase
{
    public interface IDbExecutor
    {
        TResult Query<TResult>(Func<EfContext, TResult> contextFunc);
        void Execute(Action<EfContext> contextFunc);
    }

    public class DbExecutor : IDbExecutor
    {
        public TResult Query<TResult>(Func<EfContext, TResult> contextFunc)
        {
            using(var context = new EfContext())
            {
                return contextFunc(context);
            }
        }

        public void Execute(Action<EfContext> contextFunc)
        {
            using (var context = new EfContext())
            {
                contextFunc(context);
            }
        }
    }
}
