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

    public interface IRepository
    {
        T Get<T>(int id) where T : class, IEntity, new();
        void Save<T>(T entity) where T : class, IEntity, new();
        void Delete<T>(int id) where T : class, IEntity, new();
        IEnumerable<T> Find<T>() where T : class, IEntity, new();
        IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity, new();
        IEnumerable<T> Find<T>(ICriteria criteria) where T : class, IEntity, new();
    }
}