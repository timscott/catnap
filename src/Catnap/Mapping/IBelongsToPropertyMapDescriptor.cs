using System;

namespace Catnap.Mapping
{
    public interface IBelongsToPropertyMapDescriptor
    {
        Type EntityType { get; }
        Type PropertyType { get; }
        string PropertyName { get; }
    }
}