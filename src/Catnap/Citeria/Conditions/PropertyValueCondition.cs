using System;
using System.Linq.Expressions;
using Catnap.Mapping;

namespace Catnap.Citeria.Conditions
{
    public abstract class PropertyValueCondition<T> : ValueCondition where T : class, new()
    {
        private readonly Expression<Func<T, object>> property;

        protected PropertyValueCondition(Expression<Func<T, object>> property, string format, object value) : base(format, value)
        {
            this.property = property;
        }

        public IDbCommandSpec ToCommandSpec(IEntityMap<T> entityMap, string parameterName)
        {
            var columnName = entityMap.GetColumnNameForProperty(property);
            return ToCommandSpec(columnName, parameterName);
        }
    }
}