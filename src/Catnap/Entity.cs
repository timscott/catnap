namespace Catnap
{
    public class Entity
    {
        private int? cachedHashCode;

        public virtual int Id { get; private set; }

        public bool IsTransient
        {
            get { return Id == 0; }
        }

        public bool Equals(Entity x, Entity y)
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

        public int GetHashCode(Entity obj)
        {
            return obj.IsTransient
                ? base.GetHashCode()
                : obj.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(this, obj as Entity);
        }

        public override int GetHashCode()
        {
            if (!cachedHashCode.HasValue)
            {
                cachedHashCode = GetHashCode(this);
            }
            return cachedHashCode.Value;
        }

        public static bool operator ==(Entity x, Entity y)
        {
            return object.Equals(x, y);
        }

        public static bool operator !=(Entity x, Entity y)
        {
            return !(x == y);
        }

        public void SetId(int id)
        {
            Id = id;
        }
    }
}