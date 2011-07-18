using System.Collections.Generic;
using System.Data;

namespace Catnap.Database
{
    public class DbCommandSpec
    {
        private readonly List<Parameter> parameters = new List<Parameter>();

        public DbCommandSpec SetCommandText(string value, params object[] args)
        {
            CommandText = args == null 
                ? value 
                : string.Format(value, args);
            return this;
        }

        public string CommandText { get; private set; }

        public IEnumerable<Parameter> Parameters
        {
            get { return parameters; }
        }

        public DbCommandSpec AddParameter(string name, object value)
        {
            parameters.Add(new Parameter(name, value));
            return this;
        }

        public DbCommandSpec AddParameters(params Parameter[] parms)
        {
            parameters.AddRange(parms);
            return this;
        }

        public IDbCommand CreateCommand(IDbAdapter dbAdapter, IDbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = dbAdapter.FormatCommandText(CommandText);
            foreach (var parameter in Parameters)
            {
                var p = parameter.CreateDbParameter(command, dbAdapter);
                command.Parameters.Add(p);
            }
            return command;
        }
    }
}