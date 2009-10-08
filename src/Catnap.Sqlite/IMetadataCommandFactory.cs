using Catnap.Common;

namespace Catnap.Sqlite
{
    public interface IMetadataCommandFactory
    {
        DbCommandSpec GetGetTableMetadataCommand(string tableName);
    }
}