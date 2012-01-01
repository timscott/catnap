using Catnap.Database;

namespace Catnap.Citeria.Conditions
{
    public abstract class ColumnCondition : IConditionMarker
    {
        private readonly string columnName;
        protected string format;
        public object value;

        protected ColumnCondition(string columnName, string format, object value)
        {
            this.columnName = columnName;
            this.format = format;
            this.value = value;
        }

        public Parameter ToParameter(string paramterName)
        {
            return new Parameter(paramterName, value);
        }

        public string ToSql(string parameterName)
        {
            return string.Format("({0})", string.Format(format, columnName, parameterName));
        }
    }
}