namespace Catnap.Maps
{
    public interface IIdPropertyMap<in TEntity> : IPropertyMapWithColumn<TEntity> 
        where TEntity : class, new()
    {
        void SetValue(TEntity entity, object id);
    }
}