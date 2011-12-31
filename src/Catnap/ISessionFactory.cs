namespace Catnap
{
    public interface ISessionFactory
    {
        ISession New();
    }
}