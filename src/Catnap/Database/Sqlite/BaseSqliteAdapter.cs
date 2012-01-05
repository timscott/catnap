using System;
using System.Data;
using System.Reflection;

namespace Catnap.Database.Sqlite
{
    public abstract class BaseSqliteAdapter : BaseDbAdapter, IDbAdapter
    {
        private readonly Type connectionType;
        private readonly SqliteValueConverter typeConverter = new SqliteValueConverter();

        protected BaseSqliteAdapter(Type connectionType)
        {
            this.connectionType = connectionType;
        }

        protected BaseSqliteAdapter(string connectionTypeAssemblyName, string connectionTypeName)
        {
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

        public object ConvertToDb(object value)
        {
            return typeConverter.ConvertToDb(value);
        }

        public object ConvertFromDb(object value, Type toType)
        {
            return typeConverter.ConvertFromDb(value, toType);
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
            var sql = string.Format("select * from sqlite_master where tbl_name = {0}tableName", parameterPrefix);
            return commandFactory.Create(parameters, sql);
        }
    }
}