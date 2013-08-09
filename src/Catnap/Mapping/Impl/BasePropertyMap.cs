using System;
using System.Linq.Expressions;
using System.Reflection;
using Catnap.Extensions;
using Catnap.Logging;

namespace Catnap.Mapping.Impl
{
    public abstract class BasePropertyMap<TEntity, TProperty, TConcrete> : IPropertyMap<TEntity>, IPropertyMappable<TEntity, TProperty, TConcrete>
        where TEntity : class, new()
        where TConcrete : BasePropertyMap<TEntity, TProperty, TConcrete>
    {
        protected readonly Expression<Func<TEntity, TProperty>> property;
        protected IAccessStrategy<TEntity, TProperty> accessStrategy;
        private IAccessStrategyFactory access;

        protected BasePropertyMap(string propertyName)
        {
            PropertyName = propertyName;
            // -- Added by RD for Monotouch AOT compiler limitations
            accessStrategy = new PropertyAccessStrategy<TEntity, TProperty>(propertyName);
        }

        protected BasePropertyMap(Expression<Func<TEntity, TProperty>> property)
        {
            this.property = property;
            PropertyName = property.GetMemberExpression().Member.Name;
            // -- Added by RD for Monotouch AOT compiler limitations
            accessStrategy = new PropertyAccessStrategy<TEntity, TProperty>(property);
        }

        public string PropertyName { get; private set; }

        public TConcrete Access(IAccessStrategyFactory value)
        {
            access = value;
            return (TConcrete)this;
        }

        public void SetValue(TEntity instance, object value, ISession session)
        {
            Log.Debug("Setting value '{0}' to property {1}", value, PropertyName);

            if (value == DBNull.Value)
            {
                value = null;
            }
            object convertedValue;
            try
            {
                convertedValue = session.ConvertFromDbType(value, accessStrategy.PropertyInfo.PropertyType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Failed to assign value to property: {0}.  Error converting '{1}' from type {2} to {3}",
                    accessStrategy.PropertyInfo.Name, value, value == null ? null : value.GetType(), PropertyName), ex);
            }
            try
            {
                InnerSetValue(instance, convertedValue, session);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Failed to assign the value to property '{0}'. Value is of type {1}",
                    accessStrategy.PropertyInfo.Name, convertedValue == null ? "unknown" : convertedValue.GetType().Name), ex);
            }
        }

        public PropertyInfo PropertyInfo
        {
            get { return accessStrategy.PropertyInfo; }
        }

        public virtual void Done(IDomainMap domainMap)
        {
            // -- Removed by RD due to Monotouch AOT compiler limitations, moved to constructor where type information is known.
            //access = access ?? DefaultAccess;
            //accessStrategy = property == null
            //    ? access.CreateFor<TEntity, TProperty>(PropertyName)
            //    : access.CreateFor(property);
        }

        protected virtual IAccessStrategyFactory DefaultAccess
        {
            get { return Mapping.Access.Property;  }
        }

        protected virtual void InnerSetValue(TEntity instance, object value, ISession session)
        {
            Log.Debug("Setting {0} to {1}", accessStrategy.PropertyInfo.Name, value);
            accessStrategy.Setter(instance, (TProperty)value);
        }
    }
}