using System;

namespace Catnap
{
    public class EntitySessionKey
    {
        private readonly Type type;
        private readonly object id;

        public EntitySessionKey(Type type, object id)
        {
            this.id = id;
            this.type = type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof(EntitySessionKey) && Equals((EntitySessionKey)obj);
        }

        private bool Equals(EntitySessionKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.type == type && Equals(other.id, id);
        }

        public override int GetHashCode()
        {
            unchecked { return ((type != null ? type.GetHashCode() : 0)*397) ^ (id != null ? id.GetHashCode() : 0); }
        }
    }
}