using System;
using System.Linq.Expressions;

namespace Catnap.Citeria.Conditions
{
    public class GreaterThan : LeftRightCondition
    {
        public GreaterThan(string columnName, object value) : base(columnName, ">", value) { }
    }

    public class GreaterThan<T> : LeftRightCondition<T> where T : class, new()
    {
        public GreaterThan(Expression<Func<T, object>> property, object value) : base(property, ">", value) { }
    }
}