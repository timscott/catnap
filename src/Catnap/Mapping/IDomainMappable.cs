using System;
using Catnap.Mapping.Conventions;

namespace Catnap.Mapping
{
    public interface IDomainMappable
    {
        IIdMappingConventionMappable IdConvention();
        IIdMappingConventionMappable IdConvention(Func<IEntityMapDescriptor, string> propertyNameSpec);
        void BelongsToColumnNameConvention(Func<IBelongsToPropertyMapDescriptor, string> convention);
        void ListParentIdColumnNameConvention(Func<IListPropertyMapDescriptor, string> convention);
        IEntityMappable<T> Entity<T>(Action<IEntityMappable<T>> propertyMappings) where T : class, new();
    }
}