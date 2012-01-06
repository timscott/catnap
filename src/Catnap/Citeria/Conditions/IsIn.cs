using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Catnap.Citeria.Conditions
{
    public class IsIn : ColumnValuesCondition
    {
        public IsIn(string columnName, IEnumerable<object> values)
            : base(columnName, string.Format("{{0}} in({{1}})"), values) { }
    }

    public class IsIn<T> : PropertyValuesCondition<T> where T : class, new()
    {
        public IsIn(Expression<Func<T, object>> property, IEnumerable<object> values)
            : base(property, string.Format("{{0}} in({{1}})"), values) { }
    }

    public class IsNotIn : ColumnValuesCondition
    {
        public IsNotIn(string columnName, IEnumerable<object> values)
            : base(columnName, string.Format("{{0}} not in({{1}})"), values) { }
    }

    public class IsNotIn<T> : PropertyValuesCondition<T> where T : class, new()
    {
        public IsNotIn(Expression<Func<T, object>> property, IEnumerable<object> values)
            : base(property, string.Format("{{0}} not in({{1}})"), values) { }
    }
}