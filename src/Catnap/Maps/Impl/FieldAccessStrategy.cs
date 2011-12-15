using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Catnap.Maps.Impl
{
    public abstract class FieldAccessStrategy<TEntity, TProperty> : AccessStrategy<TEntity, TProperty>
        where TEntity : class, new()
    {
        protected FieldAccessStrategy(string propertyName) : base(propertyName)
        {
            Init();
        }

        protected FieldAccessStrategy(Expression<Func<TEntity, TProperty>> property) : base(property)
        {
            Init();
        }

        private void Init()
        {
            var fieldName = GetFieldName();
            var type = typeof(TEntity);
            FieldInfo fieldInfo = null;
            while (type != null && fieldInfo == null)
            {
                fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                type = type.BaseType;
            }
            if (fieldInfo == null)
            {
                throw new Exception(string.Format("Failed to map property '{0}'. Expected field '{1}' was not found."));
            }
            Getter = entity => (TProperty)fieldInfo.GetValue(entity);
            Setter = (entity, value) => fieldInfo.SetValue(entity, value);
        }

        protected abstract string GetFieldName();

        protected string PropertyToCamelCase()
        {
            var propertyName = PropertyInfo.Name;
            return propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
        }
    }

    public class CamelCaseFieldAccessStrategy<TEntity, TProperty> : FieldAccessStrategy<TEntity, TProperty> where TEntity : class, new()
    {
        public CamelCaseFieldAccessStrategy(string propertyName) : base(propertyName) { }

        public CamelCaseFieldAccessStrategy(Expression<Func<TEntity, TProperty>> property) : base(property) { }

        protected override string GetFieldName()
        {
            return PropertyToCamelCase();
        }
    }

    public class CamelCaseUnderscoreFieldAccessStrategy<TEntity, TProperty> : FieldAccessStrategy<TEntity, TProperty> where TEntity : class, new()
    {
        public CamelCaseUnderscoreFieldAccessStrategy(string propertyName) : base(propertyName) { }

        public CamelCaseUnderscoreFieldAccessStrategy(Expression<Func<TEntity, TProperty>> property) : base(property) { }

        protected override string GetFieldName()
        {
            return "_" + PropertyToCamelCase();
        }
    }
}