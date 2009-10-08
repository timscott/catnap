using System;
using System.Linq.Expressions;

namespace Catnap.Extensions
{
    public static class ExpressionExtensions
    {
        public static MemberExpression GetMemberExpression<T, TMember>(this Expression<Func<T, TMember>> propertyExpression)
        {
            MemberExpression memberExpression;
            if (propertyExpression.Body is UnaryExpression)
            {
                memberExpression = ((UnaryExpression)propertyExpression.Body).Operand as MemberExpression;
            }
            else
            {
                memberExpression = propertyExpression.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException(string.Format("Cannot resolve a MemberExpression for propertyExpression '{0}'.", propertyExpression), "propertyExpression");
            }
            return memberExpression;
        }
    }
}