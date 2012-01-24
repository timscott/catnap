using System;
using System.Data;

namespace Catnap.Database
{
    public class NullDbAdapter : BaseDbAdapter
    {
        public override IDbConnection CreateConnection(string connectionString)
        {
            return null;
        }

        public override IDbCommand CreateLastInsertIdCommand(string tableName, IDbCommandFactory commandFactory)
        {
            return null;
        }

        public override IDbCommand CreateGetTableMetadataCommand(string tableName, IDbCommandFactory commandFactory)
        {
            return null;
        }

        public override object ConvertToDb(object value)
        {
            return value == null ? null : value.GetType();
        }

        public override object ConvertFromDb(object value, Type toType)
        {
            return value == null ? null : value.GetType();
        }

        public override string GetGeneralStringType()
        {
            return "";
        }

        public string FormatCommandText(string sql)
        {
            return sql;
        }
    }
}