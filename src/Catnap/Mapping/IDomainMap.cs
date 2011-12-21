using System;
using Catnap.Mapping.Conventions;
using Catnap.Mapping.Conventions.Impl;

namespace Catnap.Mapping
{
    public interface IDomainMap
    {
        IEntityMap<T> GetMapFor<T>() where T : class, new();
        IEntityMap GetMapFor(Type type);
        IdMappingConvention IdMappingConvention { get; }
        BelongsToColumnNameConvention BelongsToColumnNameMappingConvention { get; }
        ListParentIdColumnNameConvention ListParentIdColumnNameMappingConvention { get; }
    }
}