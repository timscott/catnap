using Catnap.Common;

namespace Catnap.Sqlite
{
    public class SqliteMetadataCommandFactory : IMetadataCommandFactory
    {
        public DbCommandSpec GetGetTableMetadataCommand()
        {
            return new DbCommandSpec()
                .SetCommandText("select * from sqlite_master");
        }

        public DbCommandSpec GetGetTableMetadataCommand(string tableName)
        {
            return new DbCommandSpec()
                .SetCommandText("select * from sqlite_master where tbl_name = @tableName")
                .AddParameter("@tableName", tableName);
        }
    }
}