namespace Catnap.Citeria.Conditions
{
    public abstract class ValueCondition : IConditionMarker
    {
        private readonly string format;
        private readonly object value;

        protected ValueCondition(string format, object value)
        {
            this.format = format;
            this.value = value;
        }

        protected IDbCommandSpec ToCommandSpec(string columnName, string parameterName)
        {
            var sql = string.Format("({0})", string.Format(format, columnName, parameterName));
            return new DbCommandSpec().SetCommandText(sql).AddParameter(parameterName, value);
        }
    }
}