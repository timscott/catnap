using System;

namespace Catnap.Maps
{
    public interface IListPropertyMap
    {
        bool IsLazy { get; }
        bool WillCascadeSaves { get; }
        bool WillCascadeDeletes { get; }
        Type ItemType { get; }
        void SetListMap(IEntityMap map);
    }

    public interface IListPropertyMap<in TEntity> : IListPropertyMap, IPropertyMap<TEntity> 
        where TEntity : class, new()
    {
        void Cascade(ISession session, TEntity parent);
    }
}