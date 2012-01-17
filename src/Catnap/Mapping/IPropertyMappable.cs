namespace Catnap.Mapping
{
    public interface IPropertyMappable<TEntity, TProperty, out TConcrete>
        where TEntity : class, new()
        where TConcrete : IPropertyMappable<TEntity, TProperty, TConcrete>
    {
        TConcrete Access(IAccessStrategyFactory value);
    }
}