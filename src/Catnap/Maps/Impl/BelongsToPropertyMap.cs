using System;
using System.Linq.Expressions;
using Catnap.Common.Logging;

namespace Catnap.Maps.Impl
{
    public class BelongsToPropertyMap<TEntity, TProperty> : BasePropertyMap<TEntity, TProperty>, IPropertyMapWithColumn<TEntity>
        where TEntity : class, IEntity, new()
        where TProperty : class, IEntity, new()
    {
        public BelongsToPropertyMap(Expression<Func<TEntity, TProperty>> property, string columnName) : base(property)
        {
            Log.Debug("Setting column name for BelongsTo property");
            ColumnName = columnName ?? MemberExpression.Member.Name;
        }

        public string ColumnName { get; private set; }

        public object GetColumnValue(IEntity instance)
        {
            var parent = getter.Invoke(instance, null);
            return parent == null
                       ? null
                       : (int?)((IEntity)(parent)).Id;
        }

        protected override void InnerSetValue(TEntity instance, object value, ISession session)
        {
            value = value == null
                        ? default(TProperty)
                        : GetEntity(session, (int)value);
            setter.Invoke(instance, new [] { value });
        }

        public TProperty GetEntity(ISession session, int id)
        {
            return session.Get<TProperty>(id);
        }
    }
}