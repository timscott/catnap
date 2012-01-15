namespace Catnap.Mapping
{
    public interface IPropertyMap<in TEntity> : IPropertyMapDescriptor where TEntity : class, new()
    {
        void SetValue(TEntity instance, object value, ISession session);
        void Done(IDomainMap domainMap);
    }
}