namespace Catnap.Find.Conditions
{
    public class GreaterThanOrEqual : LeftRightCondition
    {
        public GreaterThanOrEqual(string columnName, object value) : base(columnName, value) { }

        protected override string Operator
        {
            get { return ">="; }
        }
    }
}