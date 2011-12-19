using System;

namespace Catnap.Maps
{
    public interface IDomainMappable
    {
        IIdMappingConventionMappable IdConvention();
        IIdMappingConventionMappable IdConvention(string propertyName);
        IBelongsToColumnNameConventionMappable BelongsToColumnNameConvention(Func<IBelongsToPropertyMap, string> convention);
        IEntityMappable<T> Entity<T>(Action<IEntityMappable<T>> propertyMappings) where T : class, new();
    }
}