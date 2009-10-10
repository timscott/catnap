using System;

namespace Catnap.Extensions
{
    public static class TypeExtensions
    {
        public static object ChangeType(this object value, Type toType)
        {
            if (value == null)
            {
                return null;
            }
            toType = GetUnderlyingGenericType(toType);
            return Convert.ChangeType(value, toType);
        }

        //NOTE: other conversions needed?
        public static object ConvertValueForSqlite(this object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is bool?)
            {
                return ((bool?)value).Value ? 1 : 0;
            }
            var underlyingTpye = value.GetType().GetUnderlyingGenericType();
            if (underlyingTpye.IsEnum)
            {
                return (int) value;
            }
            if (value is DateTime)
            {
                return ((DateTime) value).Ticks;
            }
            if (value is DateTime?)
            {
                return ((DateTime?)value).Value.Ticks;
            }
            return value;
        }

        public static Type GetUnderlyingGenericType(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type;
            }
            var genericType = type.GetGenericTypeDefinition();
            return genericType.Equals(typeof(Nullable<>)) 
                       ? type.GetGenericArguments()[0] 
                       : type;
        }
    }
}