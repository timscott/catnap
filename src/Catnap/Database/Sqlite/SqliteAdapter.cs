using System;

namespace Catnap.Database.Sqlite
{
    public class SqliteAdapter : BaseSqliteAdapter
    {
        public SqliteAdapter(Type connectionType) : base(connectionType) { }

        public SqliteAdapter() :  base("System.Data.SQLite", "System.Data.SQLite.SQLiteConnection") { }
    }
}