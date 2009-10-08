using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Catnap.Common;
using Catnap.Maps;

namespace Catnap
{
    public class DbCommandPredicate<T> where T : class, IEntity, new()
    {
        private readonly IList<string> conditions = new List<string>();
        private readonly IList<Parameter> parameters = new List<Parameter>();
        private int parameterNumber;

        public IList<string> Conditions
        {
            get { return conditions; }
        }

        public IList<Parameter> Parameters
        {
            get { return parameters; }
        }

        public DbCommandPredicate<T> AddCondition(Expression<Func<T, bool>> predicate)
        {
            var sql = new StringBuilder();
            Visit(sql, predicate, false);
            Conditions.Add(sql.ToString());
            return this;
        }

        public DbCommandPredicate<T> AddCondition(string columnName, string @operator, object value)
        {
            if (value != null)
            {
                Conditions.Add(string.Format("{0}{1}@{0}", columnName, @operator));
                Parameters.Add(new Parameter(string.Format("@{0}", columnName), value));
            }
            return this;
        }

        private void Visit(StringBuilder sql, Expression expression, bool isOnRightSide)
        {
            if (expression is LambdaExpression)
            {
                Visit(sql, ((LambdaExpression)expression).Body, false);
            }
            else if (expression is BinaryExpression)
            {
                Visit(sql, (BinaryExpression)expression);
            }
            else if (expression is MemberExpression)
            {
                VisitMember(sql, (MemberExpression)expression, isOnRightSide);
            }
            else if (expression is ConstantExpression)
            {
                Visit(sql, (ConstantExpression)expression);
            }
            else if (expression is UnaryExpression)
            {
                Visit(sql, ((UnaryExpression)expression).Operand, isOnRightSide);
            }
            else
            {
                throw new NotSupportedException(string.Format("The '{0}' is not supported!", expression.GetType().Name));
            }
        }

        private void Visit(StringBuilder sql, ConstantExpression expression)
        {
            AppendValue(sql, expression.Value);
        }

        private void Visit(StringBuilder sql, ConstantExpression expression, string memberName)
        {
            const BindingFlags types = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var fieldInfo = expression.Value.GetType().GetField(memberName, types);
            if (fieldInfo != null)
            {
                var value = fieldInfo.GetValue(expression.Value);
                AppendValue(sql, value);
            }
            else
            {
                var propertyInfo = expression.Value.GetType().GetProperty(memberName, types);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(expression.Value, null);
                    AppendValue(sql, value);
                }
            }
        }

        private void Visit(StringBuilder sql, BinaryExpression expression)
        {
            sql.Append("(");
            Visit(sql, expression.Left, false);
            sql.Append(string.Format(" {0} ", GetOperandFromExpression(expression)));
            Visit(sql, expression.Right, true);
            sql.Append(")");
        }

        private void VisitMember(StringBuilder sql, MemberExpression expression, bool isOnRightSide)
        {
            if (isOnRightSide)
            {
                AppendRightSideMember(sql, expression);
            }
            else
            {
                var columnName = DomainMap.GetMapFor<T>().GetColumnNameForProperty(expression);
                sql.Append(columnName);
            }
        }

        private void AppendRightSideMember(StringBuilder sql, MemberExpression expression)
        {
            if (expression.Expression is ConstantExpression)
            {
                Visit(sql, (ConstantExpression)expression.Expression, expression.Member.Name);
            }
                //else if (expression.Expression == null)
                //{
                //var value = Expression.Lambda(expression).Compile().DynamicInvoke();
                //AppendValue(sql, value);
                //}
            else if (expression.Expression is MemberExpression)
            {
                Visit(sql, expression.Expression, true);
            }
            else
            {
                throw new ApplicationException(string.Format("Cannot process expression '{0}'", expression));

                //var value = Expression.Lambda(expression.Expression).Compile().DynamicInvoke();
                //AppendValue(sql, value);

                //var memberExpression = (MemberExpression)expression.Expression;
                //const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                //var fieldInfo = memberExpression.Type.GetField(expression.Member.Name, flags);
                //if (fieldInfo != null)
                //{
                //    var value = fieldInfo.GetValue(instance);
                //    AppendValue(sql, value);
                //}
                //else
                //{
                //    var propertyInfo = memberExpression.Type.GetProperty(expression.Member.Name, flags);
                //    if (propertyInfo != null)
                //    {
                //        var value = propertyInfo.GetValue(instance, null);
                //        AppendValue(sql, value);
                //    }
                //}
            }  
        }

        //NOTE: other conversions needed?
        private object ConvertValue(object value)
        {
            if (value is bool)
            {
                return (bool) value ? 1 : 0;
            }
            if (value.GetType().IsEnum)
            {
                return (int) value;
            }
            return value;
        }

        private void AppendValue(StringBuilder sql, object value)
        {
            var parameterName = "@" + parameterNumber;
            sql.Append(parameterName);
            Parameters.Add(new Parameter(parameterName, ConvertValue(value)));
            parameterNumber++;
        }

        private string GetOperandFromExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return "and";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.ExclusiveOr:
                    return "or";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Not:
                    return "not";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                default:
                    throw new ApplicationException(string.Format("Cannot interpret '{0}' as an operator", expression));
            }
        }
    }
}