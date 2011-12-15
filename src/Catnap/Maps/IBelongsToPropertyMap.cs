using System;

namespace Catnap.Maps
{
    public interface IBelongsToPropertyMap
    {
        void SetPropertyMap(IEntityMap map);
        Type PropertyType { get; }
    }
}