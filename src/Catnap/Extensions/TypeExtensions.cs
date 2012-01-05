using System;

namespace Catnap.Extensions
{
    public static class TypeExtensions
    {
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