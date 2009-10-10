namespace Catnap.Find.Conditions
{
    public abstract class LeftRightCondition : ColumnCondition
    {
        protected LeftRightCondition(string columnName, object value) : base(columnName, value) { }

        protected abstract string Operator { get; }

        protected override string Format
        {
            get { return string.Format("{{0}} {0} {{1}}", Operator); }
        }
    }
}