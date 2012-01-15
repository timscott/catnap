using System;
using System.Linq.Expressions;
using Catnap.Mapping.Impl;

namespace Catnap.Mapping
{
    public static class Access
    {
        public static IAccessStrategyFactory Property = new PropertyAccess();
        public static IAccessStrategyFactory CamelCaseField = new CamelCaseFieldAccess();
        public static IAccessStrategyFactory CamelCaseUnderscoreField = new CamelCaseUnderscoreFieldAccess();

        public class PropertyAccess : IAccessStrategyFactory
        {
            public IAccessStrategy<TEntity, TProperty> CreateFor<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
                where TEntity : class, new()
            {
                return new PropertyAccessStrategy<TEntity, TProperty>(property);
            }

            public IAccessStrategy<TEntity, TProperty> CreateFor<TEntity, TProperty>(string propertyName)
                where TEntity : class, new()
            {
                return new PropertyAccessStrategy<TEntity, TProperty>(propertyName);
            }
        }

        public class CamelCaseFieldAccess : IAccessStrategyFactory
        {
            public IAccessStrategy<TEntity, TProperty> CreateFor<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
                where TEntity : class, new()
            {
                return new CamelCaseFieldAccessStrategy<TEntity, TProperty>(property);
            }

            public IAccessStrategy<TEntity, TProperty> CreateFor<TEntity, TProperty>(string propertyName)
                where TEntity : class, new()
            {
                return new CamelCaseFieldAccessStrategy<TEntity, TProperty>(propertyName);
            }
        }

        public class CamelCaseUnderscoreFieldAccess : IAccessStrategyFactory
        {
            public IAccessStrategy<TEntity, TProperty> CreateFor<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
                where TEntity : class, new()
            {
                return new CamelCaseFieldAccessStrategy<TEntity, TProperty>(property);
            }

            public IAccessStrategy<TEntity, TProperty> CreateFor<TEntity, TProperty>(string propertyName)
                where TEntity : class, new()
            {
                return new CamelCaseFieldAccessStrategy<TEntity, TProperty>(propertyName);
            }
        }
    }
}