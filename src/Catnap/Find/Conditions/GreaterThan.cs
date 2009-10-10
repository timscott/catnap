namespace Catnap.Find.Conditions
{
    public class GreaterThan : LeftRightCondition
    {
        public GreaterThan(string columnName, object value) : base(columnName, value) { }

        protected override string Operator 
        {
            get { return ">"; }
        }
    }
}