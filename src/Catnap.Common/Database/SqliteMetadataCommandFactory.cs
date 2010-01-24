namespace Catnap.Common.Database
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

        public DbCommandSpec GetLastInsertIdCommand()
        {
            return new DbCommandSpec().SetCommandText("select last_insert_rowid()");
        }
    }
}