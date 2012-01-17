using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Mapping.Impl;

namespace Catnap.Mapping
{
    public interface IEntityMappable<T> : IEntityMap where T : class, new()
    {
        IEntityMappable<T> Map(IPropertyMap<T> propertyMap);
        IIdPropertyMappable<T, TProperty, IdPropertyMap<T, TProperty>> Id<TProperty>(Expression<Func<T, TProperty>> property);
        IPropertyWithColumnMappable<T, TProperty, ValuePropertyMap<T, TProperty>> Property<TProperty>(Expression<Func<T, TProperty>> property);
        IListPropertyMappable<T, TListMember> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property) where TListMember : class, new();
        IPropertyMappable<T, TProperty, BelongsToPropertyMap<T, TProperty>> BelongsTo<TProperty>(Expression<Func<T, TProperty>> property) where TProperty : class, new();
        IEntityMappable<T> Table(string tableName);
    }
}