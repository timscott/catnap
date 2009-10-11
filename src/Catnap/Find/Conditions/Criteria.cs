using System.Collections.Generic;
using Catnap.Common.Database;

namespace Catnap.Find.Conditions
{
    public class Criteria : ICriteria
    {
        private And and;
        private int parameterCount;
        private IList<Parameter> parameters = new List<Parameter>();
        private string sql;

        public Criteria(params ICondition[] conditions)
        {
            and = new And(conditions);
        }

        public ICriteria Add(params ICondition[] conditions)
        {
            and.Add(conditions);
            return this;
        }

        public ICriteria Build()
        {
            sql = Visit(and);
            return this;
        }

        public override string ToString()
        {
            return sql;
        }

        public IList<Parameter> Parameters
        {
            get { return parameters; }
        }

        private string Visit(ICondition condition)
        {
            if (condition is ColumnCondition)
            {
                return Visit((ColumnCondition) condition);
            }
            if (condition is And)
            {
                return Visit((And) condition);
            }
            if (condition is Or)
            {
                return Visit((Or)condition);
            }
            return null;
        }

        private string Visit(ColumnCondition condition)
        {
            var parameterName = "@" + parameterCount;
            parameterCount++;
            parameters.Add(new Parameter(parameterName, condition.Value));
            return condition.ToString(parameterName);
        }

        private string Visit<T>(Junction<T> junction) where T : Junction<T>
        {
            var innerSql = new List<string>();
            foreach (var condition in junction.Conditions)
            {
                innerSql.Add(Visit(condition));
            }
            return string.Format("({0})", string.Join(string.Format(" {0} ", junction.Operator), innerSql.ToArray()));
        }
    }
}