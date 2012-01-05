using System;

namespace Catnap.Database.Sqlite
{
    public class SqliteAdapter : BaseSqliteAdapter
    {
        public SqliteAdapter() : this(new SqliteValueConverter()) { }

        public SqliteAdapter(Type connectionType) : base(new SqliteValueConverter(), connectionType) { }

        public SqliteAdapter(IDbValueConverter typeConverter) : 
            base(typeConverter, "System.Data.SQLite", "System.Data.SQLite.SQLiteConnection") { }
    }
}