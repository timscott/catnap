using System;
using System.Data;

namespace Catnap.Database
{
    public interface IDbAdapter
    {
        IDbConnection CreateConnection(string connectionString);
        DbCommandSpec CreateLastInsertIdCommand();
        DbCommandSpec CreateGetTableMetadataCommand();
        DbCommandSpec CreateGetTableMetadataCommand(string tableName);
        object ConvertToDbType(object value);
        object ConvertFromDbType(object value, Type toType);
        string FormatCommandText(string sql);
        string FormatParameterName(string name);
    }
}