using System;
using System.Data;

namespace Catnap.Database
{
    public class NullDbAdapter : BaseDbAdapter, IDbAdapter
    {
        public IDbConnection CreateConnection(string connectionString)
        {
            return null;
        }

        public IDbCommand CreateLastInsertIdCommand(string tableName, IDbCommandFactory commandFactory)
        {
            return null;
        }

        public IDbCommand CreateGetTableMetadataCommand(IDbCommandFactory commandFactory)
        {
            return null;
        }

        public IDbCommand CreateGetTableMetadataCommand(string tableName, IDbCommandFactory commandFactory)
        {
            return null;
        }

        public object ConvertToDb(object value)
        {
            return value == null ? null : value.GetType();
        }

        public object ConvertFromDb(object value, Type toType)
        {
            return value == null ? null : value.GetType();
        }

        public string FormatCommandText(string sql)
        {
            return sql;
        }
    }
}