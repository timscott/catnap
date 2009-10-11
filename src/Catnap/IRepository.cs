using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Find.Conditions;

namespace Catnap
{
    public interface IRepository<T> where T : class, IEntity, new()
    {
        T Get(int id);
        void Save(T entity);
        void Delete(int id);
        IEnumerable<T> Find();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Find(ICriteria criteria);
    }
}