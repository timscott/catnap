using Catnap.Database;
using Catnap.Database.SqlServerCe;
using Catnap.Database.Sqlite;

namespace Catnap
{
    public static class DbAdapter
    {
        private static IDbAdapter sqlite;
        private static IDbAdapter sqlServerCe;

        /// <summary>
        /// An IDbAdapter for System.Data.Sqlite. Use this only when the assembly is local, otherwise instantiate SqliteAdapter passing in the connection type.
        /// </summary>
        public static IDbAdapter Sqlite
        {
            get { return sqlite = sqlite ?? new SqliteAdapter(); }
        }

        /// <summary>
        /// An IDdAdapter for System.Data.SqlServerCe. Use this only when the assembly is local, otherwise instantiate SqlServerCeAdapter passing in the connection type.
        /// </summary>
        public static IDbAdapter SqlServerCe
        {
            get { return sqlServerCe = sqlServerCe ?? new SqlServerCeAdapter(); }
        }
    }
}