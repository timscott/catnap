using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Catnap.Maps
{
    public interface IEntityMappable<T> : IEntityMap where T : class, IEntity, new()
    {
        IEntityMappable<T> Property<TProperty>(Expression<Func<T, TProperty>> property);
        IEntityMappable<T> Property<TProperty>(Expression<Func<T, TProperty>> property, string columnName);
        IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property) where TListMember : class, IEntity, new();
        IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy) where TListMember : class, IEntity, new();
        IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy, bool cascadeSaves, bool cascaseDeletes) where TListMember : class, IEntity, new();
        IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy, bool cascadeSaves, bool cascaseDeletes, Expression<Func<TListMember, bool>> filter) where TListMember : class, IEntity, new();
        IEntityMappable<T> BelongsTo<TPropertyType>(Expression<Func<T, TPropertyType>> property, string columnName) where TPropertyType : class, IEntity, new();
        IEntityMappable<T> ParentColumn(string parentColumnName);
        IEntityMappable<T> Table(string tableName);
    }
}