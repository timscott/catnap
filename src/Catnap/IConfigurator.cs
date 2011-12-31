using System;
using Catnap.Database;
using Catnap.Mapping;

namespace Catnap
{
    public interface IConfigurator
    {
        IConfigurator Domain(Action<IDomainMappable> config);
        IConfigurator ConnectionString(string connectionString);
        IConfigurator DatabaseAdapter(IDbAdapter adapter);
        ISessionFactory Build();
    }
}