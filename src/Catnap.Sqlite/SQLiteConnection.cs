using System;
using System.Data;
using System.Data.SQLite;
using Catnap.Common.Database;
using Catnap.Common.Logging;
using AdoSqliteConnection=System.Data.SQLite.SQLiteConnection;
using AdoSqliteCommand = System.Data.SQLite.SQLiteCommand;
using IDbCommand = Catnap.Common.Database.IDbCommand;
using IDbConnection = Catnap.Common.Database.IDbConnection;

namespace Catnap.Sqlite
{
    public class SqliteConnection : IDbConnection
    {
        private readonly AdoSqliteConnection adoConnection;
        private IDbTransaction transaction;

        public SqliteConnection(string connectionString)
        {
            ConnectionString = connectionString;
            adoConnection = new SQLiteConnection(connectionString);
        }

        public string ConnectionString { get; private set; }

        public void Open()
        {
            adoConnection.Open();
            Log.Debug("Connection opened");
        }

        public IDbCommand CreateCommand(DbCommandSpec commandSpec)
        {
            if (adoConnection.State == ConnectionState.Closed)
            {
                throw new SqliteException("Cannot create commands from a closed connection");
            }
            return new SqliteCommand(adoConnection, commandSpec);
        }

        public void BeginTransaction()
        {
            transaction = adoConnection.BeginTransaction();
        }

        public void RollbackTransaction()
        {
            if (transaction != null)
            {
                transaction.Rollback();
            }
        }

        public int GetLastInsertId()
        {
            using (var command = new AdoSqliteCommand("SELECT last_insert_rowid()", adoConnection))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Dispose()
        {
            if (adoConnection.State == ConnectionState.Open)
            {
                if (transaction != null)
                {
                    transaction.Commit();
                }
                adoConnection.Close();
                Log.Debug("Connection closed");
            }
        }
    }
}