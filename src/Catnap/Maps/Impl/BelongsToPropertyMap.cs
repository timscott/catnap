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

        public BelongsToPropertyMap(Expression<Func<TEntity, TProperty>> property, string columnName) : base(property)
        {
            Log.Debug("Setting column name for BelongsTo property '{0}'", property);
            ColumnName = columnName ?? MemberExpression.Member.Name;
        }

        public string ColumnName { get; private set; }

        public object GetColumnValue(object instance)
        {
            var parent = getter.Invoke(instance, null);
            return parent == null
                ? null
                : entityMap.GetId(parent);
        }

        protected override void InnerSetValue(TEntity instance, object value, ISession session)
        {
            value = value == null
                ? default(TProperty)
                : GetEntity(session, value);
            setter.Invoke(instance, new [] { value });
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