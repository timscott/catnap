namespace Catnap.Find.Conditions
{
    public class NotEqual : LeftRightCondition
    {
        public NotEqual(string columnName, object value) : base(columnName, value) { }

        protected override string Operator
        {
            get { return "!="; }
        }
    }
}