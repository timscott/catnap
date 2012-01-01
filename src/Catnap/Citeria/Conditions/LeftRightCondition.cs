using System;
using System.Linq.Expressions;

namespace Catnap.Citeria.Conditions
{
    public class LeftRightCondition : ColumnCondition
    {
        public LeftRightCondition(string columnName, string @operator, object value) 
            : base(columnName, string.Format("{{0}} {0} {{1}}", @operator), value) { }
    }

    public class LeftRightCondition<T> : PropertyCondition<T> where T : class, new()
    {
        public LeftRightCondition(Expression<Func<T, object>> property, string @operator, object value) 
            : base(property, string.Format("{{0}} {0} {{1}}", @operator), value) { }
    }
}