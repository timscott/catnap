namespace Catnap.Maps
{
    public interface IPropertyMapWithColumn<in TEntity> : IPropertyMap<TEntity>
        where TEntity : class, new()
    {
        string ColumnName { get; }
        object GetColumnValue(object instance);
    }
}