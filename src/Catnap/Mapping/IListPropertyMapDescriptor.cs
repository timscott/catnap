using System;

namespace Catnap.Mapping
{
    public interface IListPropertyMapDescriptor
    {
        Type ItemType { get; }
        Type ParentType { get; }
    }
}