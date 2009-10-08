using System;

namespace Catnap.Common
{
    public interface IDbConnection : IDisposable
    {
        void Open();
        IDbCommand CreateCommand(DbCommandSpec commandSpec);
        void BeginTransaction();
        void RollbackTransaction();
        int GetLastInsertId();
    }
}