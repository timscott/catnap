using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Catnap.Mapping.Impl;

namespace Catnap.Database.Sqlite
{
    public abstract class BaseSqliteAdapter : IDbAdapter
    {
        private const string PARAMETER_PREFIX = "@";
        private readonly Type connectionType;
        private readonly IDbTypeConverter typeConverter;

        protected BaseSqliteAdapter(IDbTypeConverter typeConverter, Type connectionType)
        {
            this.typeConverter = typeConverter;
            this.connectionType = connectionType;
        }

        protected BaseSqliteAdapter(IDbTypeConverter typeConverter, string connectionTypeAssemblyName, string connectionTypeName)
        {
            this.typeConverter = typeConverter;
            connectionType = Type.GetType(string.Format("{0},{1}", connectionTypeName, connectionTypeAssemblyName));
            if (connectionType != null)
            {
                return;
            }
            var assembly = Assembly.Load(connectionTypeAssemblyName);
            if (assembly == null)
            {
                throw new TypeLoadException(string.Format("Could not load assembly: {0}.  Are you missing a reference?", connectionTypeAssemblyName));
            }
            connectionType = assembly.GetType(connectionTypeName, true);
        }

        public IDbConnection CreateConnection(string connectionString)
        {
            if (connectionType == null)
            {
                throw new InvalidOperationException("Connection type is unknown.");
            }
            return (IDbConnection)Activator.CreateInstance(connectionType, new object[] { connectionString });
        }

        public object ConvertToDbType(object value)
        {
            return typeConverter.ConvertToDbType(value);
        }

        public object ConvertFromDbType(object value, Type toType)
        {
            return typeConverter.ConvertFromDbType(value, toType);
        }

        public string FormatParameterName(string name)
        {
            return name.StartsWith(PARAMETER_PREFIX)
                ? name 
                : PARAMETER_PREFIX + name;
        }

        public IDbCommand CreateLastInsertIdCommand(string tableName, IDbCommandFactory commandFactory)
        {
            return commandFactory.Create(null, "SELECT last_insert_rowid()");
        }

        public IDbCommand CreateGetTableMetadataCommand(IDbCommandFactory commandFactory)
        {
            return commandFactory.Create(null, "select * from sqlite_master");
        }

        public IDbCommand CreateGetTableMetadataCommand(string tableName, IDbCommandFactory commandFactory)
        {
            var parameters = new[] { new Parameter("tableName", tableName) };
            var sql = string.Format("select * from sqlite_master where tbl_name = {0}tableName", PARAMETER_PREFIX);
            return commandFactory.Create(parameters, sql);
        }
    }
}