using System;
using Catnap.Adapters.Sqlite;
using Catnap.Database;

namespace Catnap.Adapters
{
    public class SqliteAdapter : BaseSqliteAdapter
    {
        public SqliteAdapter() : this(new SqliteTypeConverter()) { }

        public SqliteAdapter(Type connectionType) : base(new SqliteTypeConverter(), connectionType) { }

        public SqliteAdapter(IDbTypeConverter typeConverter) : 
            base(typeConverter, "System.Data.SQLite", "System.Data.SQLite.SQLiteConnection") { }
    }
}