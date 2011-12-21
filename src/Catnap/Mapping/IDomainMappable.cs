using System;
using Catnap.Mapping.Conventions;

namespace Catnap.Mapping
{
    public interface IDomainMappable
    {
        IIdMappingConventionMappable IdConvention();
        IIdMappingConventionMappable IdConvention(string propertyName);
        void BelongsToColumnNameConvention(Func<IBelongsToPropertyMap, string> convention);
        void ListParentIdColumnNameConvention(Func<IListPropertyMap, string> convention);
        IEntityMappable<T> Entity<T>(Action<IEntityMappable<T>> propertyMappings) where T : class, new();
    }
}