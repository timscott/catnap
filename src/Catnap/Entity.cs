namespace Catnap
{
    public class Entity
    {
        private int? cachedHashCode;
        private int id;

        public virtual int Id
        {
            get { return id; }
        }

        public virtual bool IsTransient
        {
            get { return Id == default(int); }
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
            if (x.Id == 0 && y.Id == 0)
            {
                return ReferenceEquals(x, y);
            }
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(Entity obj)
        {
            return obj.Id == 0
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

        public virtual void SetId(int id)
        {
            this.id = id;
        }
    }
}