using System;
using Catnap.Common.Database;

namespace Catnap.Sqlite
{
    public class SqliteTypeConverter : IDbTypeConverter
    {
        //NOTE: other conversions needed?
        public object ConvertToDbType(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is bool?)
            {
                return ((bool?)value).Value ? 1 : 0;
            }
            if (value is DateTime)
            {
                return ((DateTime)value).Ticks;
            }
            if (value is DateTime?)
            {
                return ((DateTime?)value).Value.Ticks;
            }
            if (value is TimeSpan)
            {
                return ((TimeSpan)value).Ticks;
            }
            if (value is TimeSpan?)
            {
                return ((TimeSpan?)value).Value.Ticks;
            }
            var underlyingType = GetUnderlyingGenericType(value.GetType());
            if (underlyingType.IsEnum)
            {
                return (int)value;
            }
            return value;
        }

        //NOTE: other conversions needed?
        public object ConvertFromDbType(object value, Type toType)
        {
            if (value == null || !toType.IsValueType)
            {
                return value;
            }
            var fromType = value.GetType();
            if (fromType == toType)
            {
                return value;
            }
            var underlyingType = GetUnderlyingGenericType(toType);
            if (underlyingType == typeof(bool))
            {
                return ((int)value == 1);
            }
            if (underlyingType == typeof(DateTime))
            {
                return new DateTime((long)value);
            }
            if (underlyingType == typeof(TimeSpan))
            {
                return new TimeSpan((long)value);
            }
            if (underlyingType.IsEnum)
            {
                return fromType.IsEnum 
                    ? value 
                    : Enum.ToObject(underlyingType, value);
            }
            return Convert.ChangeType(value, underlyingType);
        }

        private static Type GetUnderlyingGenericType(Type type)
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