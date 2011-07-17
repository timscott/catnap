using System;
using System.Collections.Generic;
using System.Data;
using Catnap.Common.Database;
using Catnap.Common.Logging;
using AdoSqliteConnection = System.Data.SQLite.SQLiteConnection;
using AdoSqliteCommand = System.Data.SQLite.SQLiteCommand;
using IDbCommand = Catnap.Common.Database.IDbCommand;

namespace Catnap.Sqlite
{
    public class SqliteCommand : IDbCommand
    {
        private readonly SqliteTypeConverter typeConverter = new SqliteTypeConverter();
        private readonly AdoSqliteCommand adoCommand;

        public SqliteCommand(AdoSqliteConnection connection, DbCommandSpec commandSpec)
        {
            adoCommand = new AdoSqliteCommand(commandSpec.ToString(), connection);
            foreach (var parameter in commandSpec.Parameters)
            {
                var p = adoCommand.CreateParameter();
                p.ParameterName = parameter.Name;
                p.Value = typeConverter.ConvertToDbType(parameter.Value);
                p.Direction = ParameterDirection.Input;
                adoCommand.Parameters.Add(p);
            }
        }

        public int ExecuteNonQuery()
        {
            return adoCommand.ExecuteNonQuery();
        }

        public IEnumerable<IDictionary<string, object>> ExecuteQuery()
        {
            var reader = adoCommand.ExecuteReader();
            var count = 0;
            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    row.Add(reader.GetName(i), reader[i]);
                }
                yield return row;
                count++;
            }
            Log.Debug("Returning {0} rows", count);
        }

        public T ExecuteScalar<T>()
        {
            var result = adoCommand.ExecuteScalar();
            return (T)Convert.ChangeType(result, typeof(T));
        }

        //public override string ToString()
        //{
        //    return commandSpec.ToString();
        //}
    }
}