using System;
using System.ComponentModel;

namespace Catnap.Database.Types
{
    public class GuidStringType : IType
    {
        public object ToDb(object value)
        {
            return TypeHelper.NullSafeTransform(value, x => x.ToString());
        }

        public object FromDb(object value, Type toType)
        {
            return TypeHelper.NullSafeTransform(value, x => new GuidConverter().ConvertFrom(x));
        }
    }
}