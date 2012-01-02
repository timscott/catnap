using System;
using System.Data;
using Catnap.Mapping.Impl;

namespace Catnap.Database
{
    public class NullDbAdapter : IDbAdapter
    {
        private readonly string parameterPrefix;

        public NullDbAdapter() : this("@") { }

        public NullDbAdapter(string parameterPrefix)
        {
            this.parameterPrefix = parameterPrefix;
        }

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

        public object ConvertToDbType(object value)
        {
            return value == null ? null : value.GetType();
        }

        public object ConvertFromDbType(object value, Type toType)
        {
            return value == null ? null : value.GetType();
        }

        public string FormatCommandText(string sql)
        {
            return sql;
        }

        public string FormatParameterName(string name)
        {
            return name.StartsWith(parameterPrefix)
                ? name
                : parameterPrefix + name;
        }
    }
}