using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Database;
using Catnap.Mapping;

namespace Catnap.Find.Conditions
{
    public interface ICriteria<T> where T : class, new()
    {
        IList<Parameter> Parameters { get; }
        string ToSql(IEntityMap<T> entityMap, IDbAdapter dbAdapter);
        ICriteria<T> Equal(string columnName, object value);
        ICriteria<T> Equal(Expression<Func<T, object>> property, object value);
        ICriteria<T> NotEqual(string columnName, object value);
        ICriteria<T> NotEqual(Expression<Func<T, object>> property, object value);
        ICriteria<T> Greater(string columnName, object value);
        ICriteria<T> Greater(Expression<Func<T, object>> property, object value);
        ICriteria<T> Less(string columnName, object value);
        ICriteria<T> Less(Expression<Func<T, object>> property, object value);
        ICriteria<T> GreaterOrEqual(string columnName, object value);
        ICriteria<T> GreaterOrEqual(Expression<Func<T, object>> property, object value);
        ICriteria<T> LessOrEqual(string columnName, object value);
        ICriteria<T> LessOrEqual(Expression<Func<T, object>> property, object value);
        ICriteria<T> Or(Action<ICriteria<T>> criteria);
        ICriteria<T> And(Action<ICriteria<T>> criteria);
        ICriteria<T> Add(Expression<Func<object, bool>> filter);
    }
}