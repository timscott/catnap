using System;
using System.Linq.Expressions;

namespace Catnap.Citeria.Conditions
{
    public class IsNotNull : ColumnCondition
    {
        public IsNotNull(string columnName)
            : base(columnName, string.Format("{{0}} is not NULL")) { }
    }

    public class IsNotNull<T> : PropertyCondition<T> where T : class, new()
    {
        public IsNotNull(Expression<Func<T, object>> property)
            : base(property, string.Format("{{0}} is not NULL")) { }
    }
}