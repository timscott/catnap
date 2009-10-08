namespace Catnap.Common
{
    public class Parameter
    {
        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public object Value { get; set; }
        public int Index { get; set; }
    }
}