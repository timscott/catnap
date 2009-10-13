using System;
using System.Linq.Expressions;
using Catnap.Find.Conditions;
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
            return Equal(Domain.Map.GetMapFor<T>().GetColumnNameForProperty(property), value);
        }

        public static ICondition NotEqual(string columnName, object value)
        {
            return new NotEqual(columnName, value);
        }

        public static ICondition NotEqual<T>(Expression<Func<T, object>> property, object value) where T : class, IEntity, new()
        {
            return NotEqual(Domain.Map.GetMapFor<T>().GetColumnNameForProperty(property), value);
        }

        public static ICondition Greater(string columnName, object value)
        {
            return new GreaterThan(columnName, value);
        }

        public static ICondition Greater<T>(Expression<Func<T, object>> property, object value) where T : class, IEntity, new()
        {
            return Greater(Domain.Map.GetMapFor<T>().GetColumnNameForProperty(property), value);
        }

        public static ICondition Less(string columnName, object value)
        {
            return new LessThan(columnName, value);
        }

        public static ICondition Less<T>(Expression<Func<T, object>> property, object value) where T : class, IEntity, new()
        {
            return Less(Domain.Map.GetMapFor<T>().GetColumnNameForProperty(property), value);
        }


        public static ICondition GreaterOrEqual(string columnName, object value)
        {
            return new GreaterThanOrEqual(columnName, value);
        }

        public static ICondition GreaterOrEqual<T>(Expression<Func<T, object>> property, object value) where T : class, IEntity, new()
        {
            return GreaterOrEqual(Domain.Map.GetMapFor<T>().GetColumnNameForProperty(property), value);
        }

        public static ICondition LessOrEqual(string columnName, object value)
        {
            return new LessThanOrEqual(columnName, value);
        }

        public static ICondition LessOrEqual<T>(Expression<Func<T, object>> property, object value) where T : class, IEntity, new()
        {
            return LessOrEqual(Domain.Map.GetMapFor<T>().GetColumnNameForProperty(property), value);
        }

        public static ICondition Or(params ICondition[] conditions)
        {
            return new Or(conditions);
        }

        public static ICondition And(params ICondition[] conditions)
        {
            return new And(conditions);
        }
    }
}