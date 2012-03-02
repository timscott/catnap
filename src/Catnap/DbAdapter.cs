using Catnap.Database;
using Catnap.Database.SqlServerCe;
using Catnap.Database.Sqlite;
using Catnap.Database.MySql;

namespace Catnap
{
    public static class DbAdapter
    {
        private static IDbAdapter sqlite;
        private static IDbAdapter sqlServerCe;
        private static IDbAdapter mySql;

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
        
        /// <summary>
        /// An IDdAdapter for MySQL.Data. Use this only when the assembly is local, otherwise instantiate SqlServerCeAdapter passing in the connection type.
        /// </summary>
        public static IDbAdapter MySql
        {
            get { return mySql = mySql ?? new MySqlAdapter(); }
        }
    }
}