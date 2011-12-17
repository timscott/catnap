using System;
using System.Linq.Expressions;

namespace Catnap.Maps.Impl
{
    public class ValuePropertyMap<TEntity, TProperty> : PropertyWithColumnMap<TEntity, TProperty, ValuePropertyMap<TEntity, TProperty>>
        where TEntity : class, new()
    {
        public ValuePropertyMap(Expression<Func<TEntity, TProperty>> property) : base(property) { }
    }
}