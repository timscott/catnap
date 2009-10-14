using System;
using System.Linq.Expressions;

namespace Catnap.Maps.Impl
{
    internal class ValuePropertyMap<TEntity, TProperty> : BasePropertyMap<TEntity, TProperty>, IPropertyMapWithColumn<TEntity>
        where TEntity : class, IEntity, new()
    {
        public ValuePropertyMap(Expression<Func<TEntity, TProperty>> property, string columnName) : base(property)
        {
            ColumnName = columnName ?? MemberExpression.Member.Name;
        }

        public string ColumnName { get; private set; }

        public object GetColumnValue(IEntity instance)
        {
            var value = getter.Invoke(instance, null);
            return propertyInfo.PropertyType.IsEnum
                       ? (int) value
                       : value;
        }
    }
}