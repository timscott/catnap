using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Find.Conditions;

namespace Catnap
{
    public interface IRepository<T> where T : class, new()
    {
        T Get(object id);
        void Save(T entity);
        void Delete(object id);
        IEnumerable<T> Find();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Find(ICriteria criteria);
    }

    public interface IRepository
    {
        T Get<T>(object id) where T : class, new();
        void Save<T>(T entity) where T : class, new();
        void Delete<T>(object id) where T : class, new();
        IEnumerable<T> Find<T>() where T : class, new();
        IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        IEnumerable<T> Find<T>(ICriteria criteria) where T : class, new();
    }
}