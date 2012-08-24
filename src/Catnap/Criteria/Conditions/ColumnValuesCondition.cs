using System.Collections.Generic;

namespace Catnap.Citeria.Conditions
{
    public abstract class ColumnValuesCondition : ValuesCondition
    {
        private readonly string columnName;

        protected ColumnValuesCondition(string columnName, string format, IEnumerable<object> values)
            : base(format, values)
        {
            this.columnName = columnName;
        }

        public IDbCommandSpec ToCommandSpec(params string[] parameterNames)
        {
            return ToCommandSpec(columnName, parameterNames);
        }
    }
}