using Catnap.Common.Database;

namespace Catnap.Sqlite
{
    public class SqliteConnectionFactory : IConnectionFactory
    {
        private readonly string connectionString;

        public SqliteConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection NewConnection()
        {
            return new SqliteConnection(connectionString);
        }
    }
}