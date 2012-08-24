using System;
using System.Linq.Expressions;

namespace Catnap.Citeria.Conditions
{
    public class NotEqual : LeftRightCondition
    {
        public NotEqual(string columnName, object value) : base(columnName, "!=", value) { }
    }

    public class NotEqual<T> : LeftRightCondition<T> where T : class, new()
    {
        public NotEqual(Expression<Func<T, object>> property, object value) : base(property, "!=", value) { }
    }
}