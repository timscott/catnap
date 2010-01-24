namespace Catnap
{
    public class Entity : IEntity
    {
        private int? cachedHashCode;
        private readonly EntityEqualityComaparer<IEntity> equalityComparer = new EntityEqualityComaparer<IEntity>();
        private int id;

        public virtual int Id
        {
            get { return id; }
        }

        public virtual bool IsTransient
        {
            get { return Id == default(int); }
        }

        public override bool Equals(object obj)
        {
            return equalityComparer.Equals(this, obj as IEntity);
        }

        public override int GetHashCode()
        {
            if (!cachedHashCode.HasValue)
            {
				cachedHashCode = equalityComparer.GetHashCode(this);
            }
        	return cachedHashCode.Value;
        }

        public static bool operator ==(Entity x, Entity y)
        {
            return Equals(x, y);
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