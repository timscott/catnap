using Catnap.Database.Sqlite;

namespace Catnap.Database
{
    public static class DbAdapter
    {
        public static IDbAdapter sqlite;

        public static IDbAdapter Sqlite
        {
            get { return sqlite = sqlite ?? new SqliteAdapter(); }
        }
    }
}