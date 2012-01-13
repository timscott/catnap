using System;
using System.Linq.Expressions;
using Catnap.Common.Logging;

namespace Catnap.Maps.Impl
{
    public class BelongsToPropertyMap<TEntity, TProperty> : BasePropertyMap<TEntity, TProperty>, IPropertyMapWithColumn<TEntity>, IBelongsToPropertyMap
        where TEntity : class, new()
        where TProperty : class, new()
    {
        private IEntityMap entityMap;

        public BelongsToPropertyMap(Expression<Func<TEntity, TProperty>> property) : this(property, null) { }

        public BelongsToPropertyMap(Expression<Func<TEntity, TProperty>> property, string columnName) : base(property, Access.Property)
        {
            Log.Debug("Setting column name for property '{0}'", property);
            ColumnName = columnName ?? accessStrategy.PropertyInfo.Name;
        }

        public string ColumnName { get; private set; }

        public bool Insert
        {
            get { return true; }
        }

        public object GetValue(TEntity instance)
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

        public void SetPropertyMap(IEntityMap map)
        {
            entityMap = map;
        }

        public Type PropertyType
        {
            get { return typeof(TProperty); }
        }
    }
}