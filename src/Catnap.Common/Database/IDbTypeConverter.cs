using System;

namespace Catnap.Common.Database
{
    public interface IDbTypeConverter
    {
        object ConvertFromDbType(object value, Type toType);
    }
}