using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Mapping;

namespace Catnap.Citeria.Conditions
{
    public abstract class PropertyValuesCondition<T> : ValuesCondition where T : class, new()
    {
        private readonly Expression<Func<T, object>> property;

        protected PropertyValuesCondition(Expression<Func<T, object>> property, string format, IEnumerable<object> values) : base(format, values)
        {
            this.property = property;
        }

        public IDbCommandSpec ToCommandSpec(IEntityMap<T> entityMap, params string[] parameterNames)
        {
            var columnName = entityMap.GetColumnNameForProperty(property);
            return ToCommandSpec(columnName, parameterNames);
        }
    }
}