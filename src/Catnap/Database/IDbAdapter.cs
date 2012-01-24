using System;
using System.Data;

namespace Catnap.Database
{
    public interface IDbAdapter
    {
        IDbConnection CreateConnection(string connectionString);
        IDbCommand CreateLastInsertIdCommand(string tableName, IDbCommandFactory commandFactory);
        IDbCommand CreateGetTableMetadataCommand(string tableName, IDbCommandFactory commandFactory);
        object ConvertToDb(object value);
        object ConvertFromDb(object value, Type toType);
        string FormatParameterName(string name);
        string Quote(string name);
        string GetGeneralStringType();
    }
}