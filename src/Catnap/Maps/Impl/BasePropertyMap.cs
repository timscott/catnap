using System;
using System.Linq.Expressions;
using System.Reflection;
using Catnap.Common.Logging;
using Catnap.Extensions;

namespace Catnap.Maps.Impl
{
    public abstract class BasePropertyMap<TEntity, TProperty, TConcrete> : IPropertyMap<TEntity> 
        where TEntity : class, new()
        where TConcrete : BasePropertyMap<TEntity, TProperty, TConcrete>
    {
        protected readonly string propertyName;
        protected readonly Expression<Func<TEntity, TProperty>> property;
        private Access access;
        protected AccessStrategy<TEntity, TProperty> accessStrategy;

        protected BasePropertyMap(string propertyName, Access access)
        {
            this.propertyName = propertyName;
            this.access = access;
        }

        protected BasePropertyMap(Expression<Func<TEntity, TProperty>> property, Access access)
        {
            this.property = property;
            propertyName = property.GetMemberExpression().Member.Name;
            this.access = access;
        }

        public TConcrete Access(Access value)
        {
            access = value;
            return (TConcrete)this;
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

        public virtual void Done()
        {
            accessStrategy = property == null
                ? access.GetAccessStrategyFor<TEntity, TProperty>(propertyName)
                : access.GetAccessStrategyFor(property);
        }

        protected virtual void InnerSetValue(TEntity instance, object value, ISession session)
        {
            Log.Debug("Setting {0} to {1}", accessStrategy.PropertyInfo.Name, value);
            accessStrategy.Setter(instance, (TProperty)value);
        }
    }
}