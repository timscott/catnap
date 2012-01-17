using System;
using System.Linq.Expressions;

namespace Catnap.Mapping
{
    public interface IListPropertyMappable<in TEntity, TListMember> where TEntity : class, new() where TListMember : class, new()
    {
        IListPropertyMappable<TEntity, TListMember> Lazy(bool value);
        IListPropertyMappable<TEntity, TListMember> CascadeSaves(bool value);
        IListPropertyMappable<TEntity, TListMember> CascadeDeletes(bool value);
        IListPropertyMappable<TEntity, TListMember> Filter(Expression<Func<TListMember, bool>> value);
        IListPropertyMappable<TEntity, TListMember> ParentIdColumn(string columnName);
    }
}