using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Catnap.Database;
using Catnap.Extensions;

namespace Catnap.Citeria.Conditions
{
    public static class Criteria
    {
        public static ICriteria<T> For<T>() where T : class, new()
        {
            return new Criteria<T>();
        }
    }

    public class Criteria<T> : ICriteria<T>, IConditionMarker where T : class, new()
    {
        private readonly string conjunction;
        private readonly List<IConditionMarker> conditions = new List<IConditionMarker>();
        private readonly List<Expression<Func<T, bool>>> predicates = new List<Expression<Func<T, bool>>>();
        private readonly List<Parameter> parameters = new List<Parameter>();
        private int parameterCount;
        private Parameter parameter;
        private string commandText;

        public Criteria() : this("and") { }

        internal Criteria(string conjunction)
        {
            this.conjunction = conjunction;
        }

        internal Criteria(Action<ICriteria<T>> criteria, string conjunction)
        {
            this.conjunction = conjunction;
            criteria(this);
        }

        internal Criteria(IEnumerable<IConditionMarker> conditions)
            : this("and")
        {
            this.conditions.AddRange(conditions);
        }

        public ICriteria<T> And(Action<ICriteria<T>> criteria)
        {
            if (criteria != null)
            {
                var condition = new Criteria<T>(criteria, "and");
                conditions.Add(condition);
            }
            return this;
        }

        public ICriteria<T> Or(Action<ICriteria<T>> criteria)
        {
            if (criteria != null)
            {
                var condition = new Criteria<T>(criteria, "or");
                conditions.Add(condition);
            }
            return this;
        }

        public ICriteria<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                predicates.Add(predicate);
            }
            return this;
        }

        public ICriteria<T> Where(Expression<Func<T, object>> property, string @operator, object value)
        {
            property.GuardArgumentNull("property");
            var condition = new LeftRightCondition<T>(property, @operator, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Where(string columnName, string @operator, object value)
        {
            columnName.GuardArgumentNull("columnName");
            var condition = new LeftRightCondition(columnName, @operator, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Equal(string columnName, object value)
        {
            columnName.GuardArgumentNull("columnName");
            var condition = new Equal(columnName, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Equal(Expression<Func<T, object>> property, object value)
        {
            property.GuardArgumentNull("property");
            var condition = new Equal<T>(property, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> NotEqual(string columnName, object value)
        {
            columnName.GuardArgumentNull("columnName");
            var condition = new NotEqual(columnName, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> NotEqual(Expression<Func<T, object>> property, object value)
        {
            property.GuardArgumentNull("property");
            var condition = new NotEqual<T>(property, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Greater(string columnName, object value)
        {
            columnName.GuardArgumentNull("columnName");
            var condition = new GreaterThan(columnName, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Greater(Expression<Func<T, object>> property, object value)
        {
            property.GuardArgumentNull("property");
            var condition = new GreaterThan<T>(property, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Less(string columnName, object value)
        {
            columnName.GuardArgumentNull("columnName");
            var condition = new LessThan(columnName, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Less(Expression<Func<T, object>> property, object value)
        {
            property.GuardArgumentNull("property");
            var condition = new LessThan<T>(property, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> GreaterOrEqual(string columnName, object value)
        {
            columnName.GuardArgumentNull("columnName");
            var condition = new GreaterThanOrEqual(columnName, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> GreaterOrEqual(Expression<Func<T, object>> property, object value)
        {
            property.GuardArgumentNull("property");
            var condition = new GreaterThanOrEqual<T>(property, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> LessOrEqual(string columnName, object value)
        {
            columnName.GuardArgumentNull("columnName");
            var condition = new LessThanOrEqual(columnName, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> LessOrEqual(Expression<Func<T, object>> property, object value)
        {
            property.GuardArgumentNull("property");
            var condition = new LessThanOrEqual<T>(property, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Null(string columnName)
        {
            columnName.GuardArgumentNull("columnName");
            var condition = new IsNull(columnName);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Null(Expression<Func<T, object>> property)
        {
            property.GuardArgumentNull("property");
            var condition = new IsNull<T>(property);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> NotNull(string columnName)
        {
            columnName.GuardArgumentNull("columnName");
            var condition = new IsNotNull(columnName);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> NotNull(Expression<Func<T, object>> property)
        {
            property.GuardArgumentNull("property");
            var condition = new IsNotNull<T>(property);
            conditions.Add(condition);
            return this;
        }

        public IDbCommandSpec Build(ISession session)
        {
            Build(session, parameterCount);
            return new DbCommandSpec()
                .SetCommandText(commandText)
                .AddParameters(parameters.ToArray());
        }

        private void Build(ISession session, int currentParameterCount)
        {
            parameterCount = currentParameterCount;
            var conditionSqls = conditions.Select(x => Visit(x, session)).ToList();
            foreach (var predicate in predicates)
            {
                var builder = new CriteriaPredicateBuilder<T>(session);
                builder.Build(predicate, parameterCount);
                parameterCount = builder.LastParameterNumber;
                conditionSqls.Add(builder.Sql);
                parameters.AddRange(builder.Parameters);
            }
            commandText = string.Format("({0})",
                string.Join(string.Format(" {0} ", conjunction.Trim()), conditionSqls.ToArray()));
        }

        private string Visit(IConditionMarker condition, ISession session)
        {
            var columnValueCondition = condition as ColumnValueCondition;
            if (columnValueCondition != null)
            {
                return Visit(columnValueCondition, session);
            }
            var propertyValueCondition = condition as PropertyValueCondition<T>;
            if (propertyValueCondition != null)
            {
                return Visit(propertyValueCondition, session);
            }
            var columnCondition = condition as ColumnCondition;
            if (columnCondition != null)
            {
                return Visit(columnCondition);
            }
            var propertyCondition = condition as PropertyCondition<T>;
            if (propertyCondition != null)
            {
                return Visit(propertyCondition, session);
            }
            var criteriaCondition = condition as Criteria<T>;
            if (criteriaCondition != null)
            {
                return Visit(criteriaCondition, session);
            }
            return null;
        }

        private string Visit(Criteria<T> condition, ISession session)
        {
            condition.Build(session, parameterCount);
            parameterCount = condition.parameterCount;
            parameters.AddRange(condition.parameters);
            return condition.commandText;
        }

        private string Visit(ColumnValueCondition condition, ISession session)
        {
            var parameterName = session.FormatParameterName(parameterCount.ToString());
            parameterCount++;
            parameter = condition.ToParameter(parameterName);
            parameters.Add(parameter);
            return condition.ToSql(parameterName);
        }

        private string Visit(PropertyValueCondition<T> condition, ISession session)
        {
            var parameterName = session.FormatParameterName(parameterCount.ToString());
            parameterCount++;
            parameter = condition.ToParameter(parameterName);
            parameters.Add(parameter);
            return condition.ToSql(session.GetEntityMapFor<T>(), parameterName);
        }


        private string Visit(ColumnCondition condition)
        {
            return condition.ToSql();
        }

        private string Visit(PropertyCondition<T> condition, ISession session)
        {
            return condition.ToSql(session.GetEntityMapFor<T>());
        }
    }
}