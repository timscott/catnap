using System;
using System.Linq.Expressions;
using System.Reflection;
using Catnap.Common.Logging;

namespace Catnap.Maps.Impl
{
    public abstract class BasePropertyMap<TEntity, TProperty> : IPropertyMap<TEntity> 
        where TEntity : class, new()
    {
        protected readonly IAccessStrategy<TEntity, TProperty> accessStrategy;

        protected BasePropertyMap(string propertyName, IAccessStrategyFactory access)
        {
            accessStrategy = access.GetAccessStrategyFor<TEntity, TProperty>(propertyName);
        }

        protected BasePropertyMap(Expression<Func<TEntity, TProperty>> property, IAccessStrategyFactory access)
        {
            accessStrategy = access.GetAccessStrategyFor(property);
        }

        public void SetValue(TEntity instance, object value, ISession session)
        {
            Log.Debug("Setting value '{0}' to property {1}", value, accessStrategy.PropertyInfo.Name);

            if (value == DBNull.Value)
            {
                value = null;
            }
            try
            {
                value = session.ConvertFromDbType(value, accessStrategy.PropertyInfo.PropertyType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Failed to assign value to property: {0}.  Error converting '{1}' from type {2} to {3}",
                    accessStrategy.PropertyInfo.Name, value, value == null ? null : value.GetType(), accessStrategy.PropertyInfo.PropertyType.Name), ex);
            }
            try
            {
                InnerSetValue(instance, value, session);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Failed to assign the value to property '{0}'. Value is of type {1}",
                    accessStrategy.PropertyInfo.Name, value == null ? "unknown" : value.GetType().Name), ex);
            }
        }

        public PropertyInfo PropertyInfo
        {
            get { return accessStrategy.PropertyInfo; }
        }

        protected virtual void InnerSetValue(TEntity instance, object value, ISession session)
        {
            Log.Debug("Setting {0} to {1}", accessStrategy.PropertyInfo.Name, value);
            accessStrategy.Setter(instance, (TProperty)value);
        }
    }
}