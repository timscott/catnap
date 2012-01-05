using System;
using Catnap.Extensions;

namespace Catnap.Database.Types
{
    public class DefaultType : IType
    {
        public object ToDb(object value)
        {
            if (value == null)
            {
                return null;
            }
            var underlyingType = value.GetType().GetUnderlyingGenericType();
            return underlyingType.IsEnum
                ? (int)value
                : value;
        }

        public object FromDb(object value, Type toType)
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
            var underlyingType = toType.GetUnderlyingGenericType();
            return underlyingType.IsEnum 
                ? TypeHelper.NullSafeTransform(value, x => Enum.ToObject(toType, value))
                : Convert.ChangeType(value, toType);
        }
    }
}