using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Maps.Impl;

namespace Catnap.Maps
{
    public interface IEntityMappable<T> : IEntityMap where T : class, new()
    {
        IEntityMappable<T> Map(IPropertyMap<T> propertyMap);
        IEntityMappable<T> Id<TProperty>(Expression<Func<T, TProperty>> property);
        IEntityMappable<T> Id<TProperty>(Expression<Func<T, TProperty>> property, IAccessStrategyFactory access);
        IEntityMappable<T> Id<TProperty>(Expression<Func<T, TProperty>> property, IAccessStrategyFactory access, IIdValueGenerator generator);
        IEntityMappable<T> Id<TProperty>(Expression<Func<T, TProperty>> property, string columnName, IAccessStrategyFactory access, IIdValueGenerator generator);
        IEntityMappable<T> Property<TProperty>(Expression<Func<T, TProperty>> property);
        IEntityMappable<T> Property<TProperty>(Expression<Func<T, TProperty>> property, string columnName);
        IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property) where TListMember : class, new();
        IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy) where TListMember : class, new();
        IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy, bool cascadeSaves, bool cascaseDeletes) where TListMember : class, new();
        IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy, bool cascadeSaves, bool cascaseDeletes, Expression<Func<TListMember, bool>> filter) where TListMember : class, new();
        IEntityMappable<T> BelongsTo<TPropertyType>(Expression<Func<T, TPropertyType>> property, string columnName) where TPropertyType : class, new();
        IEntityMappable<T> ParentColumn(string parentColumnName);
        IEntityMappable<T> Table(string tableName);
    }
}