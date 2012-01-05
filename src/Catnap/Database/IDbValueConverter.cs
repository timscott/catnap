using System;

namespace Catnap.Database
{
    public interface IDbValueConverter
    {
        object ConvertFromDb(object value, Type toType);
        object ConvertToDb(object value);
    }
}