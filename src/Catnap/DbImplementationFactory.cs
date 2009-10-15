using Catnap.Common.Database;
using Catnap.Sqlite;

namespace Catnap
{
    public class DbImplementationFactory
    {
        public static IDbConnection NewConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }

        public static IDbTypeConverter NewTypeConverter()
        {
            return new SqliteTypeConverter();
        }
    }
}