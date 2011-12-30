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
        IEnumerable<T> List();
        IEnumerable<T> List(Expression<Func<T, bool>> predicate);
        IEnumerable<T> List(ICriteria<T> criteria);
    }

    public interface IRepository
    {
        T Get<T>(object id) where T : class, new();
        void Save<T>(T entity) where T : class, new();
        void Delete<T>(object id) where T : class, new();
        IEnumerable<T> List<T>() where T : class, new();
        IEnumerable<T> List<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        IEnumerable<T> List<T>(ICriteria<T> criteria) where T : class, new();
    }
}