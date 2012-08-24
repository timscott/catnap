using System;
using System.Collections.Generic;
using System.Linq;
using Catnap.Database;

namespace Catnap.Citeria.Conditions
{
    public abstract class ValuesCondition : IConditionMarker
    {
        private readonly string format;
        private readonly object[] values;

        protected ValuesCondition(string format, IEnumerable<object> values)
        {
            this.format = format;
            this.values = values.ToArray();
        }

        public int ValuesCount
        {
            get { return values.Count(); }
        }

        protected IDbCommandSpec ToCommandSpec(string columnName, params string[] parameterNames)
        {
            if (parameterNames.Length != values.Count())
            {
                throw new ArgumentException(string.Format("The count of parameters must match the count of values.  Parameters: {0} Values: {1}", parameterNames.Length, values.Count()));
            }
            var sql = string.Format("({0})", string.Format(format, columnName, string.Join(",", parameterNames)));
            var parameters = parameterNames.Select((t, i) => new Parameter(t, values[i]));
            return new DbCommandSpec()
                .SetCommandText(sql)
                .AddParameters(parameters);
        }
    }
}