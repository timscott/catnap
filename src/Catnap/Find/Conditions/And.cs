namespace Catnap.Find.Conditions
{
    public class And : Junction<And>
    {
        public And(params ICondition[] conditions) : base(conditions) { }

        public override string Operator
        {
            get { return "and"; }
        }
    }
}