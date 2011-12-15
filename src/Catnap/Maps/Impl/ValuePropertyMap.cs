using System;
using System.Linq.Expressions;
using Catnap.Common.Logging;

namespace Catnap.Maps.Impl
{
    public class ValuePropertyMap<TEntity, TProperty> : BasePropertyMap<TEntity, TProperty>, IPropertyMapWithColumn<TEntity>
        where TEntity : class, new()
    {
        public ValuePropertyMap(Expression<Func<TEntity, TProperty>> property) : this(property, null) { }

        public ValuePropertyMap(Expression<Func<TEntity, TProperty>> property, string columnName) : base(property)
        {
            Log.Debug("Setting column name for Value property '{0}'.", property);
            ColumnName = columnName ?? MemberExpression.Member.Name;
        }

        public string ColumnName { get; private set; }

        public object GetColumnValue(object instance)
        {
            return getter.Invoke(instance, null);
        }
    }
}