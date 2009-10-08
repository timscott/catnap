using System.Collections.Generic;

namespace Catnap
{
    public class EntityEqualityComaparer<T> : IEqualityComparer<T> where T : class, IEntity
    {
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
            if (x.IsTransient && y.IsTransient)
            {
                return ReferenceEquals(x, y);
            }
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(T obj)
        {
            if (obj.IsTransient)
            {
                return base.GetHashCode();
            }
            return obj.Id.GetHashCode();
        }
    }
}