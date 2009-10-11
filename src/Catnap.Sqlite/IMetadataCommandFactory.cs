using Catnap.Common.Database;

namespace Catnap.Sqlite
{
    public interface IMetadataCommandFactory
    {
        DbCommandSpec GetGetTableMetadataCommand(string tableName);
    }
}