using System.Data;

namespace Catnap.Common.Database
{
    public interface IConnectionFactory
    {
        IDbConnection NewConnection();
    }
}