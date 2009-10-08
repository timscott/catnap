using System.Collections.Generic;

namespace Catnap.Common
{
    public interface IDbCommand
    {
        int ExecuteNonQuery();
        IEnumerable<IDictionary<string, object>> ExecuteQuery();
    }
}