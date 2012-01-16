namespace Catnap
{
    public interface ISessionCache
    {
        void Store<T>(object id, T entity) where T : class, new();
        T Retrieve<T>(object id) where T : class, new();
    }
}