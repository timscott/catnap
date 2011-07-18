using Catnap.Database;

namespace Catnap
{
    public static class SessionFactory
    {
        public const string DEFAULT_SQL_PARAMETER_PREFIX = "@";
        
        public static void Initialize(string connectionString, IDbAdapter dbAdapter)
        {
            ConnectionString = connectionString;
            DbAdapter = dbAdapter;
        }

        public static string ConnectionString { get; private set; }
        public static IDbAdapter DbAdapter { get; private set; }

        public static Session New()
        {
            return new Session(ConnectionString, DbAdapter);
        }
    }
}