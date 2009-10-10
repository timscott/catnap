using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Find;
using Catnap.Find.Conditions;

namespace Catnap
{
    public interface IRepository<T> where T : class, IEntity, new()
    {
        IEnumerable<T> Find();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void Save(T entity);
        T Get(int id);
        void Delete(int id);
        IEnumerable<T> Find(ICriteria criteria);
        IEnumerable<T> Find(IFindSpec<T> findSpec);
    }
}