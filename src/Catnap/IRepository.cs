using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Citeria.Conditions;

namespace Catnap
{
    public interface IRepository<T> where T : class, new()
    {
        T Get(object id);
        void Save(T entity);
        void Delete(object id);
        IList<T> Find();
        IList<T> Find(Expression<Func<T, bool>> predicate);
        IList<T> Find(ICriteria<T> criteria);
    }

    public interface IRepository
    {
        T Get<T>(object id) where T : class, new();
        void Save<T>(T entity) where T : class, new();
        void Delete<T>(object id) where T : class, new();
        IList<T> Find<T>() where T : class, new();
        IList<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        IList<T> Find<T>(ICriteria<T> criteria) where T : class, new();
    }
}