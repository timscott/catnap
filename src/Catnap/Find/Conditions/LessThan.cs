using System;
using System.Linq.Expressions;

namespace Catnap.Find.Conditions
{
    public class LessThan : LeftRightCondition
    {
        public LessThan(string columnName, object value) : base(columnName, "<", value) { }
    }

    public class LessThan<T> : LeftRightCondition<T> where T : class, new()
    {
        public LessThan(Expression<Func<T, object>> property, object value) : base(property, "<", value) { }
    }
}