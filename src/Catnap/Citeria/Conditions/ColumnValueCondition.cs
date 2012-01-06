namespace Catnap.Citeria.Conditions
{
    public abstract class ColumnValueCondition : ValueCondition
    {
        private readonly string columnName;

        protected ColumnValueCondition(string columnName, string format, object value) : base(format, value)
        {
            this.columnName = columnName;
        }

        public IDbCommandSpec ToCommandSpec(string parameterName)
        {
            return ToCommandSpec(columnName, parameterName);
        }
    }
}