using System;
using System.Linq.Expressions;
using System.Reflection;
using Catnap.Common.Logging;
using Catnap.Extensions;

namespace Catnap.Maps.Impl
{
    public abstract class AccessStrategy<TEntity, TProperty> where TEntity : class, new()
    {
        protected AccessStrategy(string propertyName)
        {
            Log.Debug("Getting property info for property '{0}' of type '{1}'", propertyName, typeof(TEntity).Name);
            PropertyInfo = typeof(TEntity).GetProperty(propertyName);
            if (PropertyInfo == null)
            {
                throw new Exception(string.Format("Cannot resolve PropertyInfo for property expression '{0}'.", propertyName));
            }
        }

        protected AccessStrategy(Expression<Func<TEntity, TProperty>> property)
        {
            Log.Debug("Getting property info for property '{0}' of type '{1}'", property, GetType().Name);
            var memberExpression = property.GetMemberExpression();
            if (memberExpression == null)
            {
                throw new Exception(string.Format("Cannot get MemberExpression for property expression '{0}'.", property));
            }
            PropertyInfo = memberExpression.Member as PropertyInfo;
            if (PropertyInfo == null)
            {
                throw new Exception(string.Format("Cannot resolve PropertyInfo for property expression '{0}'.", property));
            }
        }

        public PropertyInfo PropertyInfo { get; private set; }
        public Func<TEntity, TProperty> Getter { get; protected set; }
        public Action<TEntity, TProperty> Setter { get; protected set; }
    }
}