namespace Catnap
{
    public class SessionCache : ISessionCache
    {
        private readonly ThreadSafeCache<EntitySessionKey, object> cache = new ThreadSafeCache<EntitySessionKey, object>();

        public void Store<T>(object id, T entity) where T : class, new()
        {
            var key = new EntitySessionKey(typeof(T), id);
            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
            if (entity != null)
            {
                cache.Add(key, entity);
            }
        }

        public T Retrieve<T>(object id) where T : class, new()
        {
            var key = new EntitySessionKey(typeof(T), id);
            return cache.Contains(key) 
                ? (T)cache.Get(key) 
                : default(T);
        }
    }
}