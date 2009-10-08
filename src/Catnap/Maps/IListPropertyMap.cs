namespace Catnap.Maps
{
    public interface IListPropertyMap<TEntity> : IPropertyMap<TEntity> 
        where TEntity : class, IEntity, new()
    {
        bool IsLazy { get; }
        bool WillCascadeSaves { get; }
        bool WillCascadeDeletes { get; }
        void Cascade(ISession session, TEntity parent);
    }
}