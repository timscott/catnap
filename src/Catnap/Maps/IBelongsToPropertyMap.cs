using System;

namespace Catnap.Maps
{
    public interface IBelongsToPropertyMap
    {
        void SetEntityMap(IEntityMap map);
        Type PropertyType { get; }
    }
}