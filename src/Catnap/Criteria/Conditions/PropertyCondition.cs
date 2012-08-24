using System;
using System.Linq.Expressions;
using Catnap.Mapping;

namespace Catnap.Citeria.Conditions
{
    public abstract class PropertyCondition<T> : IConditionMarker where T : class, new()
    {
        private readonly Expression<Func<T, object>> property;
        private readonly string format;

        protected PropertyCondition(Expression<Func<T, object>> property, string format)
        {
            this.property = property;
            this.format = format;
        }

        public string ToSql(IEntityMap<T> entityMap)
        {
            var columnName = entityMap.GetColumnNameForProperty(property);
            return string.Format("({0})", string.Format(format, columnName));
        }
    }
}