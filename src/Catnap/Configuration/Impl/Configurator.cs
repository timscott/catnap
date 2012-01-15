using System;
using Catnap.Database;
using Catnap.Mapping;
using Catnap.Mapping.Impl;
using Catnap.Extensions;

namespace Catnap.Configuration.Impl
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

        public ISessionFactory Build()
        {
            if (dbAdapter == null)
            {
                dbAdapter = new NullDbAdapter();
            }
            if (dbAdapter == null)
            {
                throw new ApplicationException("You must specify a DbAdapter before building the confiuration.");
            }
            var domainMap = new DomainMap(dbAdapter);
            if (domainConfig != null)
            {
                domainConfig(domainMap);
            }
            domainMap.Done();
            return new SessionFactory(connString, dbAdapter, domainMap);
        }
    }
}