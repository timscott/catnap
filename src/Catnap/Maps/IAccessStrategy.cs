using System;
using System.Reflection;

namespace Catnap.Maps
{
    public interface IAccessStrategy<TEntity, TProperty> where TEntity : class, new()
    {
        PropertyInfo PropertyInfo { get; }
        Func<TEntity, TProperty> Getter { get; }
        Action<TEntity, TProperty> Setter { get; }
    }
}