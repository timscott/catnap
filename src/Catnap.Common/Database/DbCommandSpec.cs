using System.Collections.Generic;

namespace Catnap.Common.Database
{
    public class DbCommandSpec
    {
        private string commandText;
        private List<Parameter> parameters = new List<Parameter>();

        public DbCommandSpec SetCommandText(string value, params object[] args)
        {
            commandText = args == null ? value : string.Format(value, args);
            return this;
        }

        public IEnumerable<Parameter> Parameters
        {
            get { return parameters; }
        }

        public DbCommandSpec AddParameter(object value)
        {
            return AddParameter(null, value);
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

        public override string ToString()
        {
            return commandText;
        }
    }
}