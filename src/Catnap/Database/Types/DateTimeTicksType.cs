using System;

namespace Catnap.Database.Types
{
    public class DateTimeTicksType : IType
    {
        public object ToDb(object value)
        {
            return TypeHelper.NullSafeTransform(value, x => ((DateTime)value).Ticks);
        }

        public object FromDb(object value, Type toType)
        {
            return TypeHelper.NullSafeTransform(value, x =>
            {
                var longValue = (long)Convert.ChangeType(value, typeof(long));
                return new DateTime(longValue);
            });
        }
    }
}