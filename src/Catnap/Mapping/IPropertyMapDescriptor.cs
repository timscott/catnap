using System.Reflection;

namespace Catnap.Mapping
{
    public interface IPropertyMapDescriptor
    {
        PropertyInfo PropertyInfo { get; }
        string PropertyName { get; }
    }
}