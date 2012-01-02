using System.Collections.Generic;
using Catnap.Database;

namespace Catnap
{
    public class DbCommandSpec : IDbCommandSpec
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
    }
}