using System;

namespace Catnap.Maps
{
    public interface IDomainMap
    {
        IEntityMap<T> GetMapFor<T>() where T : class, IEntity, new();
        IEntityMap GetMapFor(Type type);
    }
}