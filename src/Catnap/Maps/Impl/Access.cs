using System;
using System.Linq.Expressions;

namespace Catnap.Maps.Impl
{
    public abstract class Access
    {
        public static Access Property = new PropertyAccess();
        public static Access CamelCaseField = new CamelCaseFieldAccess();
        public static Access CamelCaseUnderscoreField = new CamelCaseUnderscoreFieldAccess();

        public abstract AccessStrategy<TEntity, TProperty> GetAccessStrategyFor<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
            where TEntity : class, new();

        public abstract AccessStrategy<TEntity, TProperty> GetAccessStrategyFor<TEntity, TProperty>(string propertyName)
            where TEntity : class, new();

        public class PropertyAccess : Access
        {
            public override AccessStrategy<TEntity, TProperty> GetAccessStrategyFor<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
            {
                return new PropertyAccessStrategy<TEntity, TProperty>(property);
            }

            public override AccessStrategy<TEntity, TProperty> GetAccessStrategyFor<TEntity, TProperty>(string propertyName)
            {
                return new PropertyAccessStrategy<TEntity, TProperty>(propertyName);
            }
        }

        public class CamelCaseFieldAccess : Access
        {
            public override AccessStrategy<TEntity, TProperty> GetAccessStrategyFor<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
            {
                return new CamelCaseFieldAccessStrategy<TEntity, TProperty>(property);
            }

            public override AccessStrategy<TEntity, TProperty> GetAccessStrategyFor<TEntity, TProperty>(string propertyName)
            {
                return new CamelCaseFieldAccessStrategy<TEntity, TProperty>(propertyName);
            }
        }

        public class CamelCaseUnderscoreFieldAccess : Access
        {
            public override AccessStrategy<TEntity, TProperty> GetAccessStrategyFor<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
            {
                return new CamelCaseFieldAccessStrategy<TEntity, TProperty>(property);
            }

            public override AccessStrategy<TEntity, TProperty> GetAccessStrategyFor<TEntity, TProperty>(string propertyName)
            {
                return new CamelCaseFieldAccessStrategy<TEntity, TProperty>(propertyName);
            }
        }
    }
}