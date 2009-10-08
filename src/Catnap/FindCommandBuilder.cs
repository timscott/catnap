using System;
using System.Linq.Expressions;
using Catnap.Common;
using Catnap.Maps;

namespace Catnap
{
    public class FindCommandBuilder<T> where T : class, IEntity, new()
    {
        private DbCommandPredicate<T> dbCommandPredicate;

        public FindCommandBuilder()
        {
            dbCommandPredicate = new DbCommandPredicate<T>();
        }

        public FindCommandBuilder<T> AddCondition(Expression<Func<T, bool>> predicate)
        {
            dbCommandPredicate.AddCondition(predicate);
            return this;
        }

        public FindCommandBuilder<T> AddCondition(string columnName, string @operator, object value)
        {
            dbCommandPredicate.AddCondition(columnName, @operator, value);
            return this;
        }

        public DbCommandSpec Build()
        {
            return DomainMap.GetMapFor<T>().GetFindCommand(dbCommandPredicate.Parameters, dbCommandPredicate.Conditions);
        }
    }
}