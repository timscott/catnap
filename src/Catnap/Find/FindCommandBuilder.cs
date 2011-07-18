using System;
using System.Linq.Expressions;
using Catnap.Database;
using Catnap.Find.Conditions;
using Catnap.Maps;

namespace Catnap.Find
{
    public class FindCommandBuilder<T> where T : class, IEntity, new()
    {
        private readonly IDomainMap domainMap;
        private readonly DbCommandPredicate<T> dbCommandPredicate;

        public FindCommandBuilder() : this(Domain.Map) { }

        public FindCommandBuilder(IDomainMap domainMap)
        {
            this.domainMap = domainMap;
            dbCommandPredicate = new DbCommandPredicate<T>();
        }

        public FindCommandBuilder<T> AddCondition(Expression<Func<T, bool>> predicate)
        {
            dbCommandPredicate.AddCondition(predicate);
            return this;
        }

        public FindCommandBuilder<T> AddCriteria(ICriteria criteria)
        {
            dbCommandPredicate.AddCriteria(criteria);
            return this;
        }

        public FindCommandBuilder<T> AddCondition(Expression<Func<T, object>> property, string @operator, object value)
        {
            var map = domainMap.GetMapFor<T>();
            var columnName = map.GetColumnNameForProperty(property);
            return AddCondition(columnName, @operator, value);
        }

        public FindCommandBuilder<T> AddCondition(string columnName, string @operator, object value)
        {
            dbCommandPredicate.AddCondition(columnName, @operator, value);
            return this;
        }

        public DbCommandSpec Build()
        {
            var map = domainMap.GetMapFor<T>();
            return map.GetFindCommand(dbCommandPredicate.Parameters, dbCommandPredicate.Conditions);
        }
    }
}