using System;

namespace Catnap.Mapping
{
    public interface IEntityMapDescriptor
    {
        string TableName { get; }
        Type EntityType { get; }
    }
}