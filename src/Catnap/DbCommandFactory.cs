using System.Collections.Generic;
using System.Data;
using System.Linq;
using Catnap.Database;

namespace Catnap
{
    public class DbCommandFactory : IDbCommandFactory
    {
        private readonly IDbAdapter dbAdapter;
        private readonly IDbConnection connection;

        public DbCommandFactory(IDbAdapter dbAdapter, IDbConnection connection)
        {
            this.dbAdapter = dbAdapter;
            this.connection = connection;
        }

        public IDbCommand Create(IEnumerable<Parameter> parameters, string sql)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            if (parameters != null)
            {
                var dbDataParameters = parameters.Select(parameter => parameter.CreateDbParameter(command, dbAdapter));
                foreach (var parameter in dbDataParameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }

        public IDbCommand Create(IDbCommandSpec commandSpec)
        {
            return Create(commandSpec.Parameters, commandSpec.CommandText);
        }
    }
}