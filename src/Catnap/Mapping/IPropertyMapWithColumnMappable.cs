namespace Catnap.Mapping
{
    public interface IPropertyWithColumnMappable<TEntity, TProperty, out TConcrete> : IPropertyMappable<TEntity, TProperty, TConcrete>
        where TEntity : class, new()
        where TConcrete : IPropertyWithColumnMappable<TEntity, TProperty, TConcrete>
    {
        TConcrete Column(string value);
    }
}