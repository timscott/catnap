using System;
using System.Linq.Expressions;

namespace Catnap.Citeria.Conditions
{
    public class Equal : LeftRightCondition
    {
        public Equal(string columnName, object value) : base(columnName, "=", value) { }
    }

    public class Equal<T> : LeftRightCondition<T> where T : class, new()
    {
        public Equal(Expression<Func<T, object>> property, object value) : base(property, "=", value) { }
    }
}