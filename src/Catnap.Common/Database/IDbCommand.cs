using System.Collections.Generic;

namespace Catnap.Common.Database
{
    public interface IDbCommand
    {
        int ExecuteNonQuery();
        IEnumerable<IDictionary<string, object>> ExecuteQuery();
    }
}