using System;

namespace Catnap.Database.Types
{
    public interface IType
    {
        object ToDb(object value);
        object FromDb(object value, Type toType);
    }
}