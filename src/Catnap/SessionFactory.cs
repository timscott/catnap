using Catnap.Database;
using Catnap.Mapping;

namespace Catnap
{
    public class SessionFactory : ISessionFactory
    {
        private readonly IDbAdapter dbAdapter;
        private readonly string connectionString;
        private readonly IDomainMap domainMap;

        public SessionFactory(string connectionString, IDbAdapter dbAdapter, IDomainMap domainMap)
        {
            this.connectionString = connectionString;
            this.dbAdapter = dbAdapter;
            this.domainMap = domainMap;
        }

        public ISession New()
        {
            return new Session(domainMap, connectionString, dbAdapter);
        }
    }
}