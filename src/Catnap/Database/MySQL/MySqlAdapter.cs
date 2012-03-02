using System;
using System.Data;

namespace Catnap.Database.MySql
{
    public class MySqlAdapter : BaseDbAdapter
    {
        private readonly MySqlValueConverter typeConverter = new MySqlValueConverter();

        public MySqlAdapter() : this("Mysql.Data") { }

        public MySqlAdapter(Type connectionType) : base(connectionType,"?","`","`") { }

        public MySqlAdapter(string connectionTypeAssemblyName) : this(ResolveConnectionType(connectionTypeAssemblyName)) { }

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
            return "varchar(200)";
        }

        public override IDbCommand CreateLastInsertIdCommand(string tableName, IDbCommandFactory commandFactory)
        {
            return commandFactory.Create(null, "SELECT LAST_INSERT_ID() FROM " + tableName);
        }

        public override IDbCommand CreateGetTableMetadataCommand(string tableName, IDbCommandFactory commandFactory)
        {
            var parameters = new[] { new Parameter("tableName", tableName) };
            var sql = string.Format("SELECT * FROM `information_schema`.`COLUMNS` WHERE `TABLE_NAME` = {0}tableName", parameterPrefix);
            return commandFactory.Create(parameters, sql);
        }
    }
}