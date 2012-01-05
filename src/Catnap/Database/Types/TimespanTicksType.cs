using System;

namespace Catnap.Database.Types
{
    public class TimespanTicksType : IType
    {
        public object ToDb(object value)
        {
            return TypeHelper.NullSafeTransform(value, x => ((TimeSpan)value).Ticks);
        }

        public object FromDb(object value, Type toType)
        {
            return TypeHelper.NullSafeTransform(value, x =>
            {
                var longValue = (long)Convert.ChangeType(value, typeof(long));
                return new TimeSpan(longValue);
            });
        }
    }
}