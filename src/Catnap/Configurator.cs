using System;
using Catnap.Database;
using Catnap.Mapping;

namespace Catnap
{
    public class Configurator : IConfigurator
    {
        private string connString;
        private IDbAdapter dbAdapter = new NullDbAdapter();
        private Action<IDomainMappable> domainConfig;

        public IConfigurator Domain(Action<IDomainMappable> config)
        {
            domainConfig = config;
            return this;
        }

        public IConfigurator ConnectionString(string connectionString)
        {
            connString = connectionString;
            return this;
        }

        public IConfigurator DatabaseAdapter(IDbAdapter adapter)
        {
            dbAdapter = adapter;
            return this;
        }

        public void Done()
        {
            SessionFactory.Initialize(connString, dbAdapter, domainConfig);
        }
    }
}