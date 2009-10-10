namespace Catnap.Find.Conditions
{
    public class LessThanOrEqual : LeftRightCondition
    {
        public LessThanOrEqual(string columnName, object value) : base(columnName, value) { }

        protected override string Operator
        {
            get { return "<="; }
        }
    }
}