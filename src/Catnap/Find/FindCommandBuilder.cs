using System;
using System.Linq.Expressions;
using Catnap.Database;
using Catnap.Find.Conditions;
using Catnap.Mapping;

namespace Catnap.Find
{
    public class FindCommandBuilder<T> where T : class, new()
    {
        private readonly IEntityMap<T> entityMap;
        private readonly DbCommandPredicate<T> dbCommandPredicate;

        public FindCommandBuilder() : this(SessionFactory.Current.DomainMap.GetMapFor<T>(), SessionFactory.Current.DbAdapter) { }

        public FindCommandBuilder(IEntityMap<T> entityMap, IDbAdapter dbAdapter)
        {
            this.entityMap = entityMap;
            dbCommandPredicate = new DbCommandPredicate<T>(entityMap, dbAdapter);
        }

        public FindCommandBuilder<T> AddCondition(Expression<Func<T, bool>> predicate)
        {
            dbCommandPredicate.AddCondition(predicate);
            return this;
        }

        public FindCommandBuilder<T> AddCriteria(ICriteria<T> criteria)
        {
            dbCommandPredicate.AddCriteria(criteria);
            return this;
        }

        public FindCommandBuilder<T> AddCondition(Expression<Func<T, object>> property, string @operator, object value)
        {
            var columnName = entityMap.GetColumnNameForProperty(property);
            return AddCondition(columnName, @operator, value);
        }

        public FindCommandBuilder<T> AddCondition(string columnName, string @operator, object value)
        {
            dbCommandPredicate.AddCondition(columnName, @operator, value);
            return this;
        }

        public DbCommandSpec Build()
        {
            return entityMap.GetFindCommand(dbCommandPredicate.Parameters, dbCommandPredicate.Conditions);
        }
    }
}