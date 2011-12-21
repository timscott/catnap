using System;

namespace Catnap.Maps
{
    public interface IBelongsToPropertyMap
    {
        void Done(IEntityMap map);
        Type PropertyType { get; }
        string PropertyName { get; }
    }
}