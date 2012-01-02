using System;
using System.Data;
using Catnap.Mapping.Impl;

namespace Catnap.Database
{
    public interface IDbAdapter
    {
        IDbConnection CreateConnection(string connectionString);
        IDbCommand CreateLastInsertIdCommand(string tableName, IDbCommandFactory commandFactory);
        IDbCommand CreateGetTableMetadataCommand(IDbCommandFactory commandFactory);
        IDbCommand CreateGetTableMetadataCommand(string tableName, IDbCommandFactory commandFactory);
        object ConvertToDbType(object value);
        object ConvertFromDbType(object value, Type toType);
        string FormatParameterName(string name);
    }
}