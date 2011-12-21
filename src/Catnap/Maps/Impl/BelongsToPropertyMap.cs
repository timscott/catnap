using System;
using System.Linq.Expressions;

namespace Catnap.Maps.Impl
{
    public class BelongsToPropertyMap<TEntity, TProperty> : PropertyWithColumnMap<TEntity, TProperty, BelongsToPropertyMap<TEntity, TProperty>>, IBelongsToPropertyMap
        where TEntity : class, new()
        where TProperty : class, new()
    {
        private IEntityMap entityMap;

        public BelongsToPropertyMap(Expression<Func<TEntity, TProperty>> property) : base(property) { }

        public override object GetValue(TEntity instance)
        {
            var parent = accessStrategy.Getter(instance);
            return parent == null
                ? null
                : entityMap.GetId(parent);
        }

        protected override void InnerSetValue(TEntity instance, object value, ISession session)
        {
            var entity = value == null
                ? default(TProperty)
                : GetEntity(session, value);
            accessStrategy.Setter(instance, entity);
        }

        public TProperty GetEntity(ISession session, object id)
        {
            return session.Get<TProperty>(id);
        }

        public void Done(IEntityMap map)
        {
            entityMap = map;
        }

        public Type PropertyType
        {
            get { return typeof(TProperty); }
        }

        protected override string GetDeafultColumnName(IDomainMap domainMap)
        {
            return domainMap.BelongsToColumnNameMappingConvention == null 
                ? PropertyName + "Id" 
                : domainMap.BelongsToColumnNameMappingConvention.GetColumnName(this);
        }
    }
}