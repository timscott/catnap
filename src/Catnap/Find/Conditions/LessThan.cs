namespace Catnap.Find.Conditions
{
    public class LessThan : LeftRightCondition
    {
        public LessThan(string columnName, object value) : base(columnName, value) { }

        protected override string Operator
        {
            get { return "<"; }
        }
    }
}