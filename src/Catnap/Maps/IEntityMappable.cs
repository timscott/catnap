using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Maps.Impl;

namespace Catnap.Maps
{
    public interface IEntityMappable<T> : IEntityMap where T : class, new()
    {
        IEntityMappable<T> Map(IPropertyMap<T> propertyMap);
        IdPropertyMap<T, TProperty> Id<TProperty>(Expression<Func<T, TProperty>> property);
        ValuePropertyMap<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> property);
        ListPropertyMap<T, TListMember> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property) where TListMember : class, new();
        BelongsToPropertyMap<T, TProperty> BelongsTo<TProperty>(Expression<Func<T, TProperty>> property) where TProperty : class, new();
        IEntityMappable<T> ParentColumn(string parentColumnName);
        IEntityMappable<T> Table(string tableName);
    }
}