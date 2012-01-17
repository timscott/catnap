namespace Catnap.Mapping
{
    public interface IIdPropertyMappable<TEntity, TProperty, out TConcrete> : IPropertyWithColumnMappable<TEntity, TProperty, TConcrete>
        where TEntity : class, new()
        where TConcrete : IPropertyWithColumnMappable<TEntity, TProperty, TConcrete>
    {
        TConcrete Generator(IIdValueGenerator value);
    }
}