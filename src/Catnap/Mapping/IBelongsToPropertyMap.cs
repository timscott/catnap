using System;

namespace Catnap.Mapping
{
    public interface IBelongsToPropertyMap
    {
        void Done(IEntityMap map);
        Type PropertyType { get; }
        string PropertyName { get; }
    }
}