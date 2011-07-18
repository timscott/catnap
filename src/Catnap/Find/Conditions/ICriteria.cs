using System.Collections.Generic;
using Catnap.Database;

namespace Catnap.Find.Conditions
{
    public interface ICriteria
    {
        IList<Parameter> Parameters { get; }
        ICriteria Add(params ICondition[] conditions);
        ICriteria Build();
    }
}