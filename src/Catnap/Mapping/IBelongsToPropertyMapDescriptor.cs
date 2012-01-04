using System;

namespace Catnap.Mapping
{
    public interface IBelongsToPropertyMapDescriptor
    {
        Type PropertyType { get; }
        string PropertyName { get; }
    }
}