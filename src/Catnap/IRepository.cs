using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Citeria.Conditions;

namespace Catnap
{
    [Obsolete]
    public interface IRepository<T> where T : class, new()
    {
        T Get(object id);
        void Save(T entity);
        void Delete(object id);
        IList<T> Find();
        IList<T> Find(Expression<Func<T, bool>> predicate);
        IList<T> Find(ICriteria<T> criteria);
    }
}