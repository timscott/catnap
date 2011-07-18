using System;
using System.Data;
using System.Reflection;
using Catnap.Database;

namespace Catnap.Adapters
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
                throw new InvalidOperationException("You must initialize");
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

        public string FormatCommandText(string sql)
        {
            return sql.Replace(SessionFactory.DEFAULT_SQL_PARAMETER_PREFIX, PARAMETER_PREFIX);
        }

        public string FormatParameterName(string name)
        {
            return name.StartsWith(PARAMETER_PREFIX)
                       ? name :
                                  PARAMETER_PREFIX + name;
        }

        public DbCommandSpec CreateLastInsertIdCommand()
        {
            return new DbCommandSpec().SetCommandText("SELECT last_insert_rowid()");
        }

        public DbCommandSpec CreateGetTableMetadataCommand()
        {
            return new DbCommandSpec()
                .SetCommandText("select * from sqlite_master");
        }

        public DbCommandSpec CreateGetTableMetadataCommand(string tableName)
        {
            return new DbCommandSpec()
                .SetCommandText("select * from sqlite_master where tbl_name = @tableName")
                .AddParameter("tableName", tableName);
        }
    }
}