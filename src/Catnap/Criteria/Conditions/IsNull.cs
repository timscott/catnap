using System;
using System.Linq.Expressions;

namespace Catnap.Citeria.Conditions
{
    public class IsNull : ColumnCondition
    {
        public IsNull(string columnName)
            : base(columnName, string.Format("{{0}} is NULL")) { }
    }

    public class IsNull<T> : PropertyCondition<T> where T : class, new()
    {
        public IsNull(Expression<Func<T, object>> property)
            : base(property, string.Format("{{0}} is NULL")) { }
    }
}