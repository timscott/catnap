using System;

namespace Catnap.Database.Types
{
    public class BooleanIntType : IType
    {
        public object ToDb(object value)
        {
            return TypeHelper.NullSafeTransform(value, x => ((bool?)x).Value ? 1 : 0);
        }

        public object FromDb(object value, Type toType)
        {
            return TypeHelper.NullSafeTransform(value, x => Convert.ToBoolean(x));
        }
    }
}