using Catnap.Database;

namespace Catnap
{
    public static class SessionFactory
    {
        private static IDbAdapter dbAdapter;
        private static string connectionString;

        internal static void Initialize(string connectionString, IDbAdapter dbAdapter)
        {
            SessionFactory.connectionString = connectionString;
            SessionFactory.dbAdapter = dbAdapter;
        }

        public static ISession New()
        {
            return new Session(connectionString, dbAdapter);
        }

        //TODO: This does not belong here
        internal static string FormatParameterName(string name)
        {
            const string defaultSqlParameterPrefix = "@";
            return dbAdapter == null
                ? name.StartsWith(defaultSqlParameterPrefix)
                    ? name
                    : defaultSqlParameterPrefix + name
                : dbAdapter.FormatParameterName(name);
        }
    }
}