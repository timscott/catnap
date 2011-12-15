using System;
using System.Linq.Expressions;
using Catnap.Common.Logging;
using Catnap.Extensions;

namespace Catnap.Maps.Impl
{
 public class IdPropertyMap<TEntity, TProperty> : BasePropertyMap<TEntity, TProperty>, IPropertyMapWithColumn<TEntity>, IIdPropertyMap<TEntity>
        where TEntity : class, new()
    {
        public IdPropertyMap(Expression<Func<TEntity, TProperty>> property) : this(property, Access.CamelCaseField) { }

        public IdPropertyMap(Expression<Func<TEntity, TProperty>> property, Access access) : this(property, access, false) { }

        public IdPropertyMap(Expression<Func<TEntity, TProperty>> property, Access access, bool insert) : this(property, null, access, insert) { }

        public IdPropertyMap(string propertyName, string columnName, Access access, bool insert) : base(propertyName, access)
        {
            Log.Debug("Setting column name for Value property '{0}'.", propertyName);
            ColumnName = columnName ?? accessStrategy.PropertyInfo.Name;
            Insert = insert;
        }

        public IdPropertyMap(Expression<Func<TEntity, TProperty>> property, string columnName, Access access, bool insert) : base(property, access)
        {
            Log.Debug("Setting column name for Value property '{0}'.", property.GetMemberExpression().Member.Name);
            ColumnName = columnName ?? accessStrategy.PropertyInfo.Name;
            Insert = insert;
        }

        public string ColumnName { get; private set; }

        public bool Insert { get; private set; }

        public object GetValue(TEntity instance)
        {
            return accessStrategy.Getter(instance);
        }

        public void SetValue(TEntity entity, object id)
        {
            accessStrategy.Setter(entity, (TProperty)id);
        }

        public Type PropertyType
        {
            get { return accessStrategy.PropertyInfo.PropertyType; }
        }
    }
}