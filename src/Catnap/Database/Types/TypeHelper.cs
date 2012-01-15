using System;

namespace Catnap.Database.Types
{
    public  class TypeHelper
    {
        public static object NullSafeTransform(object value, Func<object, object> conversion)
        {
            return value == null
                       ? null
                       : conversion(value);
        }
    }
}