using System;
using System.Linq.Expressions;

namespace Catnap.Maps
{
    public interface IAccessStrategyFactory
    {
        IAccessStrategy<TEntity, TProperty> GetAccessStrategyFor<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
            where TEntity : class, new();

        IAccessStrategy<TEntity, TProperty> GetAccessStrategyFor<TEntity, TProperty>(string propertyName)
            where TEntity : class, new();
    }
}