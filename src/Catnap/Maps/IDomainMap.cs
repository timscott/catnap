using System;

namespace Catnap.Maps
{
    public interface IDomainMap
    {
        IEntityMap<T> GetMapFor<T>() where T : class, new();
        IEntityMap GetMapFor(Type type);
        IdMappingConvention IdMappingConvention { get; }
    }
}