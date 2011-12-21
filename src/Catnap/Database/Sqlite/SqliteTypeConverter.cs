using System;
using System.ComponentModel;
using Catnap.Logging;

namespace Catnap.Database.Sqlite
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
            var underlyingType = GetUnderlyingGenericType(value.GetType());
            if (value is bool?)
            {
                return ((bool?)value).Value ? 1 : 0;
            }
            if (underlyingType == typeof(Guid))
            {
                return value.ToString();
            }
            if (underlyingType == typeof(DateTime))
            {
                return ((DateTime)value).Ticks;
            }
            if (underlyingType == typeof(TimeSpan))
            {
                return ((TimeSpan)value).Ticks;
            }
            if (underlyingType.IsEnum)
            {
                return (int)value;
            }
            return value;
        }

        //NOTE: other conversions needed?
        public object ConvertFromDbType(object value, Type toType)
        {
            Log.Debug("Converting '{0}' to type {1}", value, toType.FullName);
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
            if (underlyingType == typeof(Guid))
            {
                 return new GuidConverter().ConvertFrom(value);
            }
            if (underlyingType == typeof(DateTime))
            {
                var longValue = (long) Convert.ChangeType(value, typeof (long));
                return new DateTime(longValue);
            }
            if (underlyingType == typeof(TimeSpan))
            {
                var longValue = (long)Convert.ChangeType(value, typeof(long));
                return new TimeSpan(longValue);
            }
            if (underlyingType.IsEnum)
            {
                return fromType.IsEnum 
                    ? value 
                    : Enum.ToObject(underlyingType, value);
            }
            return Convert.ChangeType(value, toType);
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