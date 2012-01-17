using System;
using System.Linq.Expressions;

namespace Catnap.Mapping.Impl
{
    public abstract class PropertyWithColumnMap<TEntity, TProperty, TConcrete> : BasePropertyMap<TEntity, TProperty, TConcrete>, IPropertyMapWithColumn<TEntity>, IPropertyWithColumnMappable<TEntity, TProperty, TConcrete>
        where TEntity : class, new()
        where TConcrete : PropertyWithColumnMap<TEntity, TProperty, TConcrete>
    {
        protected PropertyWithColumnMap(string propertyName) : base(propertyName) { }

        protected PropertyWithColumnMap(Expression<Func<TEntity, TProperty>> property) : base(property) { }

        public virtual object GetValue(TEntity instance)
        {
            return accessStrategy.Getter(instance);
        }

        public string ColumnName { get; protected set; }

        public virtual bool Insert
        {
            get { return true; }
        }

        public TConcrete Column(string value)
        {
            ColumnName = value;
            return (TConcrete)this;
        }

        public override void Done(IDomainMap domainMap)
        {
            base.Done(domainMap);
            ColumnName = ColumnName ?? GetDeafultColumnName(domainMap);
        }

        protected virtual string GetDeafultColumnName(IDomainMap domainMap)
        {
            return PropertyName;
        }
    }
}