using Catnap.Find;

namespace Catnap
{
    public class Equal : BaseCondition, ICondition
    {
        public Equal(object left, object right) : base(left, right) { }

        public string Operator
        {
            get { return "="; }
        }
    }
}