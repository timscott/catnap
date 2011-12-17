namespace Catnap.Maps
{
    public interface IIdPropertyMap<in TEntity> : IPropertyMapWithColumn<TEntity>
        where TEntity : class, new()
    {
        object Generate(TEntity entity);
    }
}