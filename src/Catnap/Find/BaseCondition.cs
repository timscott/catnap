namespace Catnap.Find
{
    public class BaseCondition
    {
        public BaseCondition(object left, object right)
        {
            Left = left == null ? null : ConvertValue(left).ToString();
            Right = right == null ? null : ConvertValue(right).ToString();
        }

        public object Left { get; protected set; }
        public object Right { get; protected set; }

        //NOTE: other conversions needed?
        private object ConvertValue(object value)
        {
            if (value is bool)
            {
                return (bool)value ? 1 : 0;
            }
            if (value.GetType().IsEnum)
            {
                return (int)value;
            }
            return value;
        }
    }
}