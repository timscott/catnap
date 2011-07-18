using System;

namespace Catnap.Database
{
    public interface IDbTypeConverter
    {
        object ConvertFromDbType(object value, Type toType);
        object ConvertToDbType(object value);
    }
}