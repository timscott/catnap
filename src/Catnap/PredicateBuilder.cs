using System;
using System.Linq;
using System.Linq.Expressions;

namespace Catnap
{
    public class PredicateBuilder<T>
    {
        public static PredicateBuilder<T> New()
        {
            return new PredicateBuilder<T>();
        }

        public Expression<Func<T, bool>> Predicate { get; set; }

        public PredicateBuilder<T> Or(Expression<Func<T, bool>> or)
        {
            if (Predicate == null)
            {
                Predicate = or;
            }
            else
            {
                var invokedExpr = Expression.Invoke(or, Predicate.Parameters.Cast<Expression>());
                Predicate = Expression.Lambda<Func<T, bool>>(Expression.Or(Predicate.Body, invokedExpr), Predicate.Parameters);
            }
            return this;
        }

        public PredicateBuilder<T> And(Expression<Func<T, bool>> and)
        {
            if (Predicate == null)
            {
                Predicate = and;
            }
            else
            {
                var invokedExpr = Expression.Invoke(and, Predicate.Parameters.Cast<Expression>());
                Predicate = Expression.Lambda<Func<T, bool>>(Expression.And(Predicate.Body, invokedExpr), Predicate.Parameters);
            }
            return this;
        }

        public PredicateBuilder<T> Or(bool apply, Expression<Func<T, bool>> or)
        {
            return apply ? Or(or) : this;
        }

        public PredicateBuilder<T> And(bool apply, Expression<Func<T, bool>> and)
        {
            return apply ? And(and) : this;
        }
    }
}