using Catnap.Mapping;

namespace Catnap
{
    public interface ISessionFactory
    {
        ISession New();
        string FormatParameterName(string name);
        IDomainMap DomainMap { get; }
    }
}