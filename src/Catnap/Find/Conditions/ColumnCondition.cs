namespace Catnap.Find.Conditions
{
    public abstract class ColumnCondition : ICondition
    {
        protected ColumnCondition(string columnName, object value)
        {
            ColumnName = columnName;
            Value = value;
        }

        public object Value { get; private set; }

        public string ColumnName { get; private set; }

        protected abstract string Format { get; }

        public string ToString(string parameterName)
        {
            return string.Format("({0})", string.Format(Format, ColumnName, parameterName));
        }
    }
}