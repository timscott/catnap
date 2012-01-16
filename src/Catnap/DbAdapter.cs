using Catnap.Database;
using Catnap.Database.Sqlite;

namespace Catnap
{
    public static class DbAdapter
    {
        public static IDbAdapter sqlite;

        /// <summary>
        /// A DBAdapter for System.Data.Sqlite
        /// </summary>
        public static IDbAdapter Sqlite
        {
            get { return sqlite = sqlite ?? new SqliteAdapter(); }
        }
    }
}