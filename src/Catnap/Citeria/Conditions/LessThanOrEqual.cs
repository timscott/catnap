using System;
using System.Linq.Expressions;

namespace Catnap.Citeria.Conditions
{
    public class LessThanOrEqual : LeftRightCondition
    {
        public LessThanOrEqual(string columnName, object value) : base(columnName, "<=", value) { }
    }

    public class LessThanOrEqual<T> : LeftRightCondition<T> where T : class, new()
    {
        public LessThanOrEqual(Expression<Func<T, object>> property, object value) : base(property, "<=", value) { }
    }
}