namespace Catnap
{
    public class NullSessionCache : ISessionCache
    {
        public void Store<T>(object id, T entity) where T : class, new()
        {
        }

        public T Retrieve<T>(object id) where T : class, new()
        {
            return null;
        }
    }
}