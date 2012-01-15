using System;

namespace Catnap.Tests.Core.Models
{
    public class EntityGuid : Entity<Guid>
    {
        public override bool IsTransient
        {
            get { return Id == Guid.Empty; }
        }
    }

    public class EntityInt : Entity<int>
    {
        public override bool IsTransient
        {
            get { return Id == 0; }
        }
    }

    public abstract class Entity<T>
    {
        private int? cachedHashCode;

        public virtual T Id { get; private set; }

        public abstract bool IsTransient { get; }

        public bool Equals(Entity<T> x, Entity<T> y)
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

        public int GetHashCode(Entity<T> obj)
        {
            return obj.IsTransient
                ? base.GetHashCode()
                : obj.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(this, obj as Entity<T>);
        }

        public override int GetHashCode()
        {
            if (!cachedHashCode.HasValue)
            {
                cachedHashCode = GetHashCode(this);
            }
            return cachedHashCode.Value;
        }

        public static bool operator ==(Entity<T> x, Entity<T> y)
        {
            return object.Equals(x, y);
        }

        public static bool operator !=(Entity<T> x, Entity<T> y)
        {
            return !(x == y);
        }

        public void SetId(T id)
        {
            Id = id;
        }
    }
}