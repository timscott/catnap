using Catnap.Common.Database;
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