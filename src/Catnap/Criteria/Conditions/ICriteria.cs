using System;
using System.Linq.Expressions;

namespace Catnap.Citeria.Conditions
{
    public interface ICriteria<T> where T : class, new()
    {
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
        ICriteria<T> Null(string columnName);
        ICriteria<T> Null(Expression<Func<T, object>> property);
        ICriteria<T> NotNull(string columnName);
        ICriteria<T> NotNull(Expression<Func<T, object>> property);
        ICriteria<T> Or(Action<ICriteria<T>> criteria);
        ICriteria<T> And(Action<ICriteria<T>> criteria);
        ICriteria<T> Where(Expression<Func<T, bool>> predicate);
        ICriteria<T> Where(Expression<Func<T, object>> property, string @operator, object value);
        ICriteria<T> Where(string columnName, string @operator, object value);
        ICriteria<T> In(string columnName, params object[] values);
        ICriteria<T> In(Expression<Func<T, object>> property, params object[] values);
        ICriteria<T> NotIn(string columnName, params object[] values);
        ICriteria<T> NotIn(Expression<Func<T, object>> property, params object[] values);
        IDbCommandSpec Build(ISession session);
    }
}