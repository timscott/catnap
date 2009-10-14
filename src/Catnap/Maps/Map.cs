using Catnap.Maps.Impl;

namespace Catnap.Maps
{
    public static class Map
    {
        public static IEntityMappable<T> Entity<T>() where T : class, IEntity, new()
        {
            return new EntityMap<T>();
        }
    }
}