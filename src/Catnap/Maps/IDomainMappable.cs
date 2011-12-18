using System;

namespace Catnap.Maps
{
    public interface IDomainMappable
    {
        IDomainMappable IdConvention(IdMappingConvention convention);
        IEntityMappable<T> Entity<T>(Action<IEntityMappable<T>> propertyMappings) where T : class, new();
    }
}