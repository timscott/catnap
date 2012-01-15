namespace Catnap.Mapping
{
    public interface IPropertyMapWithColumn<in TEntity> : IPropertyMap<TEntity>
        where TEntity : class, new()
    {
        string GetColumnName();
        object GetValue(TEntity instance);
        bool Insert { get; }
    }
}