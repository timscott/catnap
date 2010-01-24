using System;
using System.Linq.Expressions;
using System.Reflection;
using Catnap.Common.Logging;
using Catnap.Extensions;

namespace Catnap.Maps.Impl
{
    public abstract class BasePropertyMap<TEntity, TProperty> : IPropertyMap<TEntity> 
        where TEntity : class, IEntity, new()
    {
        protected IDomainMap domainMap;
        protected PropertyInfo propertyInfo;
        protected MethodInfo setter;
        protected MethodInfo getter;

        protected BasePropertyMap(Expression<Func<TEntity, TProperty>> property)
        {
            Log.Debug("Getting member expression for property '{0}' of type '{1}'", property, GetType().Name);
            MemberExpression = property.GetMemberExpression();

            if (MemberExpression == null)
            {
                throw new Exception(string.Format("Cannot get MemberExpression for property expression '{0}'.", property));
            }
            propertyInfo = MemberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new Exception(string.Format("Cannot resolve PropertyInfo for property expression '{0}'.", property));
            }
            Log.Debug("Getting set method for property '{0}'.", propertyInfo.Name);
            setter = propertyInfo.GetSetMethod(true);
            if (setter == null)
            {
                throw new ArgumentException(string.Format("The property '{0}' is not writable.", propertyInfo.Name), "property");
            }
            Log.Debug("Getting set method for property '{0}'.", propertyInfo.Name);
            getter = propertyInfo.GetGetMethod(true);
            if (getter == null)
            {
                throw new ArgumentException(string.Format("The property '{0}' is not readable.", propertyInfo.Name), "property");
            }
        }

        public MemberExpression MemberExpression { get; private set; }

        public void SetValue(TEntity instance, object value, ISession session)
        {
            Log.Debug("Setting value '{0}' to property {1}", value, propertyInfo.Name);
            if (value == DBNull.Value)
            {
                value = null;
            }
            try
            {
                value = session.ConvertFromDbType(value, propertyInfo.PropertyType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Failed to assign value to property: {0}.  Error converting '{1}' from type {2} to {3}",
                    propertyInfo.Name, value, value == null ? null : value.GetType(), propertyInfo.PropertyType.Name), ex);
            }
            try
            {
                InnerSetValue(instance, value, session);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Failed to assign the value to property '{0}'. Value is of type {1}",
                    propertyInfo.Name, value == null ? "unknown" : value.GetType().Name), ex);
            }
        }

        public bool SetterIsPrivate
        {
            get { return setter.IsPrivate; }
        }

        public void SetDomainMap(IDomainMap value)
        {
            domainMap = value;
        }

        protected virtual void InnerSetValue(TEntity instance, object value, ISession session)
        {
            Log.Debug("Setting {0} to {1}", setter.Name, value);
            setter.Invoke(instance, new[] { value });
        }
    }
}