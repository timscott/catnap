namespace Catnap.Find.Conditions
{
    public class Or : Junction<And>
    {
        public Or(params ICondition[] conditions) : base(conditions) { }

        public override string Operator
        {
            get { return "or"; }
        }
    }
}