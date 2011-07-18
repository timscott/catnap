using System.Data;

namespace Catnap.Database
{
    public class Parameter
    {
        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public object Value { get; private set; }

        public IDbDataParameter CreateDbParameter(IDbCommand command, IDbAdapter dbAdapter)
        {
            var result = command.CreateParameter();
            result.ParameterName = dbAdapter.FormatParameterName(Name);
            result.Value = dbAdapter.ConvertToDbType(Value);
            return result;
        }
    }
}