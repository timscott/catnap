using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public DbCommandSpec AddParameter(Func<string, object> parm)
        {
            parameters.Add(new Parameter(parm.Method.GetParameters().First().Name, parm(null)));
            return this;
        }

        public DbCommandSpec AddParameters(params Parameter[] parms)
        {
            parameters.AddRange(parms);
            return this;
        }

        public DbCommandSpec AddParameters(IEnumerable<Parameter> parms)
        {
            parameters.AddRange(parms);
            return this;
        }

        public DbCommandSpec AddParameters(params Func<string, object>[] parms)
        {
            parameters.AddRange(parms.Select(x => new Parameter(x.Method.GetParameters().First().Name, x(null))));
            return this;
        }

        public DbCommandSpec AddParameters(IEnumerable<Func<string, object>> parms)
        {
            parameters.AddRange(parms.Select(x => new Parameter(x.Method.GetParameters().First().Name, x(null))));
            return this;
        }

        public DbCommandSpec AddParameters(object parms)
        {
            var props = parms.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            parameters.AddRange(props.Select(x => new Parameter(x.Name, x.GetValue(parms, null))));
            return this;
        }
    }
}