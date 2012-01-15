using System;
using System.Linq.Expressions;

namespace Catnap.Mapping
{
    public interface IAccessStrategyFactory
    {
        IAccessStrategy<TEntity, TProperty> CreateFor<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
            where TEntity : class, new();

        IAccessStrategy<TEntity, TProperty> CreateFor<TEntity, TProperty>(string propertyName)
            where TEntity : class, new();
    }
}