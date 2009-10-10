using System;
using System.Linq.Expressions;
using Catnap.Maps;

namespace Catnap.Find
{
    public class Condition
    {
        public static ICondition Equal(string columnName, object value)
        {
            return new Equal(columnName, value);
        }

        public static ICondition Equal<T>(Expression<Func<T, object>> property, object value) where T : class, IEntity, new()
        {
            return new Equal(DomainMap.GetMapFor<T>().GetColumnNameForProperty(property), value);
        }
    }
}