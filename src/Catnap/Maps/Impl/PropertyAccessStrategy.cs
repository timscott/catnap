using System;
using System.Linq.Expressions;
using Catnap.Common.Logging;

namespace Catnap.Maps.Impl
{
    public class PropertyAccessStrategy<TEntity, TProperty> : AccessStrategy<TEntity, TProperty> where TEntity : class, new()
    {
        public PropertyAccessStrategy(string propertyName) : base(propertyName)
        {
            Init();
        }

        public PropertyAccessStrategy(Expression<Func<TEntity, TProperty>> property) : base(property)
        {
            Init();
        }

        private void Init()
        {
            Log.Debug("Getting getter for property '{0}'.", PropertyInfo.Name);
            var getMethod = PropertyInfo.GetGetMethod(true);
            if (getMethod == null)
            {
                throw new ArgumentException(string.Format("The property '{0}' is not readable.", PropertyInfo.Name), "property");
            }
            Getter = entity => (TProperty)getMethod.Invoke(entity, null);

            Log.Debug("Getting setter for property '{0}'.", PropertyInfo.Name);
            var setMethod = PropertyInfo.GetSetMethod(true);
            if (setMethod == null)
            {
                throw new ArgumentException(string.Format("The property '{0}' is not writable.", PropertyInfo.Name), "property");
            }
            Setter = (entity, value) => setMethod.Invoke(entity, new object[] {value});
        }
    }
}