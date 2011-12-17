using System;

namespace Catnap.Maps
{
    public interface IDomainMappable
    {
        IEntityMappable<T> Entity<T>(params Func<IEntityMappable<T>, IPropertyMap<T>>[] propertyMaps) where T : class, new();
    }
}