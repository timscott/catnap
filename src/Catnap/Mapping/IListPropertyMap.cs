using System;
using System.Linq.Expressions;

namespace Catnap.Mapping
{
    public interface IListPropertyMap : IListPropertyMapDescriptor
    {
        void Done(IDomainMap domainMap, IEntityMap parentMap, IEntityMap listItemMap);
    }

    public interface IListPropertyMap<in TEntity> : IListPropertyMap, IPropertyMap<TEntity> where TEntity : class, new()
    {
        void Cascade(ISession session, TEntity parent);
    }

    public interface IListPropertyMappable<in TEntity, TListMember> where TEntity : class, new() where TListMember : class, new()
    {
        IListPropertyMappable<TEntity, TListMember> Lazy(bool value);
        IListPropertyMappable<TEntity, TListMember> CascadeSaves(bool value);
        IListPropertyMappable<TEntity, TListMember> CascadeDeletes(bool value);
        IListPropertyMappable<TEntity, TListMember> Filter(Expression<Func<TListMember, bool>> value);
        IListPropertyMappable<TEntity, TListMember> ParentIdColumn(string columnName);
    }
}