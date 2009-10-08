namespace Catnap.Maps
{
    public interface IPropertyMapWithColumn<TEntity> : IPropertyMap<TEntity>
        where TEntity : class, IEntity, new()
    {
        string ColumnName { get; }
        object GetColumnValue(IEntity instance);
    }
}