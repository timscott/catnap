using System;
using Catnap.Database;
using Catnap.Mapping;
using Catnap.Mapping.Impl;

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
            var domainMap = new DomainMap(dbAdapter);
            domainConfig(domainMap);
            domainMap.Done();
            var sessionFactory = new SessionFactory(connString, dbAdapter, domainMap);
            UnitOfWork.Initialize(sessionFactory);
            return sessionFactory;
        }
    }
}