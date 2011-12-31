using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Catnap.Database;
using Catnap.Mapping;

namespace Catnap.Find.Conditions
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
        private string sql;

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
            var condition = new Criteria<T>(criteria, "and");
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Or(Action<ICriteria<T>> criteria)
        {
            var condition = new Criteria<T>(criteria, "or");
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Where(Expression<Func<T, bool>> predicate)
        {
            predicates.Add(predicate);
            return this;
        }

        public ICriteria<T> Where(Expression<Func<T, object>> property, string @operator, object value)
        {
            var condition = new LeftRightCondition<T>(property, @operator, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Where(string columnName, string @operator, object value)
        {
            var condition = new LeftRightCondition(columnName, @operator, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Equal(string columnName, object value)
        {
            var condition = new Equal(columnName, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Equal(Expression<Func<T, object>> property, object value)
        {
            var condition = new Equal<T>(property, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> NotEqual(string columnName, object value)
        {
            var condition = new NotEqual(columnName, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> NotEqual(Expression<Func<T, object>> property, object value)
        {
            var condition = new NotEqual<T>(property, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Greater(string columnName, object value)
        {
            var condition = new GreaterThan(columnName, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Greater(Expression<Func<T, object>> property, object value)
        {
            var condition = new GreaterThan<T>(property, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Less(string columnName, object value)
        {
            var condition = new LessThan(columnName, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> Less(Expression<Func<T, object>> property, object value)
        {
            var condition = new LessThan<T>(property, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> GreaterOrEqual(string columnName, object value)
        {
            var condition = new GreaterThanOrEqual(columnName, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> GreaterOrEqual(Expression<Func<T, object>> property, object value)
        {
            var condition = new GreaterThanOrEqual<T>(property, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> LessOrEqual(string columnName, object value)
        {
            var condition = new LessThanOrEqual(columnName, value);
            conditions.Add(condition);
            return this;
        }

        public ICriteria<T> LessOrEqual(Expression<Func<T, object>> property, object value)
        {
            var condition = new LessThanOrEqual<T>(property, value);
            conditions.Add(condition);
            return this;
        }

        public IEnumerable<Parameter> Parameters
        {
            get { return parameters; }
        }

        public string Sql
        {
            get { return sql; }
        }

        public ICriteria<T> Done(IEntityMap<T> entityMap, IDbAdapter dbAdapter)
        {
            Done(entityMap, dbAdapter, parameterCount);
            return this;
        }

        private void Done(IEntityMap<T> entityMap, IDbAdapter dbAdapter, int currentParameterCount)
        {
            parameterCount = currentParameterCount;
            var conditionSqls = conditions.Select(x => Visit(x, entityMap, dbAdapter)).ToList();
            foreach (var predicate in predicates)
            {
                var builder = new CriteriaPredicateBuilder<T>(entityMap, dbAdapter);
                builder.Build(predicate, parameterCount);
                parameterCount = builder.LastParameterNumber;
                conditionSqls.Add(builder.Sql);
                parameters.AddRange(builder.Parameters);
            }
            sql = string.Format("({0})",
                string.Join(string.Format(" {0} ", conjunction.Trim()), conditionSqls.ToArray()));
        }

        private string Visit(IConditionMarker condition, IEntityMap<T> entityMap, IDbAdapter dbAdapter)
        {
            if (condition is ColumnCondition)
            {
                return Visit((ColumnCondition)condition, dbAdapter);
            }
            if (typeof(PropertyCondition<T>).IsAssignableFrom(condition.GetType()))
            {
                return Visit((PropertyCondition<T>)condition, entityMap, dbAdapter);
            }
            if (condition is Criteria<T>)
            {
                return Visit((Criteria<T>)condition, entityMap, dbAdapter);
            }
            return null;
        }

        private string Visit(Criteria<T> condition, IEntityMap<T> entityMap, IDbAdapter dbAdapter)
        {
            condition.Done(entityMap, dbAdapter, parameterCount);
            parameterCount = condition.parameterCount;
            parameters.AddRange(condition.parameters);
            return condition.sql;
        }

        private string Visit(ColumnCondition condition, IDbAdapter dbAdapter)
        {
            var parameterName = dbAdapter.FormatParameterName(parameterCount.ToString());
            parameterCount++;
            parameter = condition.ToParameter(parameterName);
            parameters.Add(parameter);
            return condition.ToSql(parameterName);
        }

        private string Visit(PropertyCondition<T> condition, IEntityMap<T> entityMap, IDbAdapter dbAdapter)
        {
            var parameterName = dbAdapter.FormatParameterName(parameterCount.ToString());
            parameterCount++;
            parameter = condition.ToParameter(parameterName);
            parameters.Add(parameter);
            return condition.ToSql(entityMap, parameterName);
        }
    }
}