namespace Catnap.Citeria.Conditions
{
    public abstract class ColumnCondition : IConditionMarker
    {
        private readonly string columnName;
        protected string format;

        protected ColumnCondition(string columnName, string format)
        {
            this.columnName = columnName;
            this.format = format;
        }

        public string ToSql()
        {
            return string.Format("({0})", string.Format(format, columnName));
        }
    }
}