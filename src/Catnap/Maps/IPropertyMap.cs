using System.Linq.Expressions;

namespace Catnap.Maps
{
    public interface IPropertyMap<in TEntity> where TEntity : class, new()
    {
        void SetValue(TEntity instance, object value, ISession session);
        bool SetterIsPrivate { get; }
        MemberExpression MemberExpression { get; }
    }
}