using Catnap.Common;
using Catnap.Sqlite;

namespace Catnap
{
    public class ConnectionFactory
    {
        public static IDbConnection New(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
    }
}