using System;
using System.Linq.Expressions;
using Catnap.Common.Logging;

namespace Catnap.Maps.Impl
{
    public abstract class PropertyWithColumnMap<TEntity, TProperty, TConcrete> : BasePropertyMap<TEntity, TProperty, TConcrete>, IPropertyMapWithColumn<TEntity>
        where TEntity : class, new()
        where TConcrete : PropertyWithColumnMap<TEntity, TProperty, TConcrete>
    {
        private string columnName;

        protected PropertyWithColumnMap(string propertyName) : base(propertyName) { }

        protected PropertyWithColumnMap(Expression<Func<TEntity, TProperty>> property) : base(property) { }

        public virtual object GetValue(TEntity instance)
        {
            return accessStrategy.Getter(instance);
        }

        public virtual bool Insert
        {
            get { return true; }
        }

        public TConcrete ColumnName(string value)
        {
            columnName = value;
            return (TConcrete)this;
        }

        public string GetColumnName()
        {
            return columnName;
        }

        public override void Done()
        {
            base.Done();
            Log.Debug("Setting column name for Value property '{0}'.", propertyName);
            ColumnName(columnName ?? accessStrategy.PropertyInfo.Name);
        }
    }
}