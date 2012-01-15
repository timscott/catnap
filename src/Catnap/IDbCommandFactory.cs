using System.Collections.Generic;
using System.Data;
using Catnap.Database;

namespace Catnap
{
    public interface IDbCommandFactory
    {
        IDbCommand Create(IEnumerable<Parameter> parameters, string sql);
        IDbCommand Create(IDbCommandSpec commandSpec);
    }
}