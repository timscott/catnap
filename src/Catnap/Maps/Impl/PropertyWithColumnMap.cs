using System;
using System.Linq.Expressions;

namespace Catnap.Maps.Impl
{
    public abstract class PropertyWithColumnMap<TEntity, TProperty, TConcrete> : BasePropertyMap<TEntity, TProperty, TConcrete>, IPropertyMapWithColumn<TEntity>
        where TEntity : class, new()
        where TConcrete : PropertyWithColumnMap<TEntity, TProperty, TConcrete>
    {
        protected string columnName;

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

        public override void Done(IDomainMap domainMap)
        {
            base.Done(domainMap);
            columnName = columnName ?? GetDeafultColumnName(domainMap);
        }

        protected virtual string GetDeafultColumnName(IDomainMap domainMap)
        {
            return PropertyName;
        }
    }
}