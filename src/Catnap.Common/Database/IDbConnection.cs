using System;

namespace Catnap.Common.Database
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