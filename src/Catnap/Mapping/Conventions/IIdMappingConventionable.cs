using System;

namespace Catnap.Mapping.Conventions
{
    public interface IIdMappingConventionMappable
    {
        IIdMappingConventionMappable Column(Func<IEntityMapDescriptor, string> columnNameSpec);
        IIdMappingConventionMappable Access(IAccessStrategyFactory access);
        IIdMappingConventionMappable Generator(IIdValueGenerator generator);
    }
}