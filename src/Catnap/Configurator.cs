using System;
using Catnap.Database;
using Catnap.Mapping;
using Catnap.Mapping.Impl;

namespace Catnap
{
    public class Configurator : IConfigurator
    {
        private string connString;
        private IDbAdapter dbAdapter;
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
            var map = new DomainMap();
            domainConfig(map);
            map.Done();
            SessionFactory.Initialize(connString, dbAdapter, map);
        }
    }
}