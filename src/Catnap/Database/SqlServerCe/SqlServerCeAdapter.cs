using System;
using System.Data;

namespace Catnap.Database.SqlServerCe
{
    public class SqlServerCeAdapter : BaseDbAdapter
    {
        private readonly SqlServerCeValueConverter typeConverter = new SqlServerCeValueConverter();

        public SqlServerCeAdapter() : this("System.Data.SqlServerCe") { }

        public SqlServerCeAdapter(Type connectionType) : base(connectionType) { }

        public SqlServerCeAdapter(string connectionTypeAssemblyName) : this(ResolveConnectionType(connectionTypeAssemblyName)) { }

        public override object ConvertToDb(object value)
        {
            return typeConverter.ConvertToDb(value);
        }

        public override object ConvertFromDb(object value, Type toType)
        {
            return typeConverter.ConvertFromDb(value, toType);
        }

        public override string GetGeneralStringType()
        {
            return "nvarchar(200)";
        }

        public override IDbCommand CreateLastInsertIdCommand(string tableName, IDbCommandFactory commandFactory)
        {
            return commandFactory.Create(null, "select @@identity");
        }

        public override IDbCommand CreateGetTableMetadataCommand(string tableName, IDbCommandFactory commandFactory)
        {
            var parameters = new[] { new Parameter("tableName", tableName) };
            var sql = string.Format("select * from information_schema.columns where table_name = {0}tableName", parameterPrefix);
            return commandFactory.Create(parameters, sql);
        }
    }
}