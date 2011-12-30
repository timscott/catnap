using System;
using System.Linq.Expressions;

namespace Catnap.Find.Conditions
{
    public abstract class LeftRightCondition : ColumnCondition
    {
        protected LeftRightCondition(string columnName, string @operator, object value) 
            : base(columnName, string.Format("{{0}} {0} {{1}}", @operator), value) { }
    }

    public abstract class LeftRightCondition<T> : PropertyCondition<T> where T : class, new()
    {
        protected LeftRightCondition(Expression<Func<T, object>> property, string @operator, object value) 
            : base(property, string.Format("{{0}} {0} {{1}}", @operator), value) { }
    }
}