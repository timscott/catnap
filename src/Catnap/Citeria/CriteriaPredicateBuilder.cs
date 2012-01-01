using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Catnap.Database;
using Catnap.Mapping;

namespace Catnap.Citeria
{
    public class CriteriaPredicateBuilder<T> where T : class, new()
    {
        private readonly IEntityMap<T> entityMap;
        private readonly IDbAdapter dbAdapter;
        private List<Parameter> parameters;
        private StringBuilder sql;
        private int parameterNumber;

        public CriteriaPredicateBuilder(IEntityMap<T> entityMap, IDbAdapter dbAdapter)
        {
            this.entityMap = entityMap;
            this.dbAdapter = dbAdapter;
            sql = new StringBuilder();
        }

        public string Sql
        {
            get { return sql.ToString(); }
        }

        public IEnumerable<Parameter> Parameters
        {
            get { return parameters; }
        }

        public int LastParameterNumber
        {
            get { return parameterNumber; }
        }

        public void Build(Expression<Func<T, bool>> predicate, int startingParamterNumber)
        {
            Reset();
            parameterNumber = startingParamterNumber;
            Visit(predicate, false);
        }

        private void Reset()
        {
            sql = new StringBuilder();
            parameters = new List<Parameter>();
        }

        private void Visit(Expression expression, bool isOnRightSide)
        {
            if (expression is LambdaExpression)
            {
                Visit(((LambdaExpression)expression).Body, false);
            }
            else if (expression is BinaryExpression)
            {
                Visit((BinaryExpression)expression);
            }
            else if (expression is MemberExpression)
            {
                VisitMember((MemberExpression)expression, isOnRightSide);
            }
            else if (expression is ConstantExpression)
            {
                Visit((ConstantExpression)expression);
            }
            else if (expression is UnaryExpression)
            {
                Visit(((UnaryExpression)expression).Operand, isOnRightSide);
            }
            else
            {
                throw new NotSupportedException(string.Format("The '{0}' is not supported!", expression.GetType().Name));
            }
        }

        private void Visit(ConstantExpression expression)
        {
            AppendValue(expression.Value);
        }

        private void Visit(ConstantExpression expression, string memberName)
        {
            const BindingFlags types = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var fieldInfo = expression.Value.GetType().GetField(memberName, types);
            if (fieldInfo != null)
            {
                var value = fieldInfo.GetValue(expression.Value);
                AppendValue(value);
            }
            else
            {
                var propertyInfo = expression.Value.GetType().GetProperty(memberName, types);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(expression.Value, null);
                    AppendValue(value);
                }
            }
        }

        private void Visit(BinaryExpression expression)
        {
            sql.Append("(");
            Visit(expression.Left, false);
            sql.Append(string.Format(" {0} ", GetOperandFromExpression(expression)));
            Visit(expression.Right, true);
            sql.Append(")");
        }

        private void VisitMember(MemberExpression expression, bool isOnRightSide)
        {
            if (isOnRightSide)
            {
                AppendRightSideMember(expression);
            }
            else
            {
                var columnName = entityMap.GetColumnNameForProperty(expression);
                sql.Append(columnName);
            }
        }

        private void AppendRightSideMember(MemberExpression expression)
        {
            if (expression.Expression is ConstantExpression)
            {
                Visit((ConstantExpression)expression.Expression, expression.Member.Name);
            }
            //else if (expression.Expression == null)
            //{
            //var value = Expression.Lambda(expression).Compile().DynamicInvoke();
            //AppendValue(sql, value);
            //}
            else if (expression.Expression is MemberExpression)
            {
                Visit(expression.Expression, true);
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
                return (bool)value ? 1 : 0;
            }
            if (value.GetType().IsEnum)
            {
                return (int)value;
            }
            return value;
        }

        private void AppendValue(object value)
        {
            var parameterName = dbAdapter.FormatParameterName(LastParameterNumber.ToString());
            sql.Append(parameterName);
            parameters.Add(new Parameter(parameterName, ConvertValue(value)));
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