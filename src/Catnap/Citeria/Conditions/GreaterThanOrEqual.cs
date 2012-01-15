using System;
using System.Linq.Expressions;

namespace Catnap.Citeria.Conditions
{
    public class GreaterThanOrEqual : LeftRightCondition
    {
        public GreaterThanOrEqual(string columnName, object value) : base(columnName, ">=", value) { }
    }

    public class GreaterThanOrEqual<T> : LeftRightCondition<T> where T : class, new()
    {
        public GreaterThanOrEqual(Expression<Func<T, object>> property, object value) : base(property, ">=", value) { }
    }
}