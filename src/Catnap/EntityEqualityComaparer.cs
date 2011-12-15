using System.Collections.Generic;
using Catnap.Maps;

namespace Catnap
{
    public class EntityEqualityComaparer<T> : IEqualityComparer<T> where T : class
    {
        private readonly IEntityMap entityMap;

        public EntityEqualityComaparer(IEntityMap entityMap)
        {
            this.entityMap = entityMap;
        }

        public bool Equals(T x, T y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return true;
            }
            if (entityMap.IsTransient(x) && entityMap.IsTransient(y))
            {
                return ReferenceEquals(x, y);
            }
            return entityMap.GetId(x).Equals(entityMap.GetId(y));
        }

        public int GetHashCode(T obj)
        {
            return entityMap.IsTransient(obj) 
                ? base.GetHashCode() 
                : entityMap.GetId(obj).GetHashCode();
        }
    }
}