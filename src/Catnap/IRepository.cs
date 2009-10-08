using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Catnap
{
    public interface IRepository<T> where T : IEntity, new()
    {
        IEnumerable<T> Find();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void Save(T entity);
        T Get(int id);
        void Delete(int id);
    }
}