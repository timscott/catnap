namespace Catnap.Mapping
{
    public interface IListPropertyMap : IListPropertyMapDescriptor
    {
        bool GetIsLazy();
        bool GetWillCascadeSaves();
        bool GetWillCascadeDeletes();
        void Done(IDomainMap domainMap, IEntityMap parentMap, IEntityMap listItemMap);
    }

    public interface IListPropertyMap<in TEntity> : IListPropertyMap, IPropertyMap<TEntity> 
        where TEntity : class, new()
    {
        void Cascade(ISession session, TEntity parent);
    }
}