using System;

namespace Catnap.Mapping
{
    public interface IListPropertyMap
    {
        bool GetIsLazy();
        bool GetWillCascadeSaves();
        bool GetWillCascadeDeletes();
        Type ItemType { get; }
        Type ParentType { get; }
        void Done(IDomainMap domainMap, IEntityMap parentMap, IEntityMap listItemMap);
    }

    public interface IListPropertyMap<in TEntity> : IListPropertyMap, IPropertyMap<TEntity> 
        where TEntity : class, new()
    {
        void Cascade(ISession session, TEntity parent);
    }
}