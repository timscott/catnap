using System;

namespace Catnap.Maps
{
    public interface IListPropertyMap
    {
        bool GetIsLazy();
        bool GetWillCascadeSaves();
        bool GetWillCascadeDeletes();
        Type ItemType { get; }
        void SetMaps(IEntityMap parentMap, IEntityMap listItemMap);
    }

    public interface IListPropertyMap<in TEntity> : IListPropertyMap, IPropertyMap<TEntity> 
        where TEntity : class, new()
    {
        void Cascade(ISession session, TEntity parent);
    }
}