using System.Linq.Expressions;

namespace Catnap.Maps
{
    public interface IPropertyMap<TEntity> where TEntity : class, IEntity, new()
    {
        void SetValue(TEntity instance, object value, ISession session);
        bool SetterIsPrivate { get; }
        MemberExpression MemberExpression { get; }
    }
}