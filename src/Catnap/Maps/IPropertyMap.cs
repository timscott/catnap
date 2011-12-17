using System.Reflection;

namespace Catnap.Maps
{
    public interface IPropertyMap<in TEntity> where TEntity : class, new()
    {
        void SetValue(TEntity instance, object value, ISession session);
        PropertyInfo PropertyInfo { get; }
        void Done();
    }
}