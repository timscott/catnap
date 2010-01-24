namespace Catnap.Common.Database
{
    public interface IMetadataCommandFactory
    {
        DbCommandSpec GetGetTableMetadataCommand(string tableName);
    }
}