using System.Collections.Generic;
using System.Linq;

namespace Catnap.Find.Conditions
{
    public abstract class Junction<T> : ICondition where T : Junction<T>
    {
        protected List<ICondition> conditions = new List<ICondition>();

        protected Junction(IEnumerable<ICondition> conditions)
        {
            if (conditions != null)
            {
                this.conditions.AddRange(conditions);
            }
        }
        
        public void Add(params ICondition[] conditionsToAdd)
        {
             if (conditionsToAdd != null)
             {
                 conditions.AddRange(conditionsToAdd);
             }
        }

        public IEnumerable<ICondition> Conditions
        {
            get { return conditions; }
        }

        public abstract string Operator { get; }
    }
}