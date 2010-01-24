namespace Catnap.Common.Database
{
    public class Parameter
    {
        public Parameter(string name, object value)
        {
            if (!name.StartsWith("@"))
            {
                name = "@" + name;
            }
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public object Value { get; set; }
        public int Index { get; set; }
    }
}