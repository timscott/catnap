namespace Catnap
{
    public interface ISessionFactory
    {
        ISession Create();
    }
}