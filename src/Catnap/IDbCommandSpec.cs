using System.Collections.Generic;
using Catnap.Database;

namespace Catnap
{
    public interface IDbCommandSpec
    {
        IEnumerable<Parameter> Parameters { get; }
        string CommandText { get; }
    }
}