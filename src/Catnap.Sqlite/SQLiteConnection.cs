using System;
using System.Linq;
using Catnap.Common;

namespace Catnap.Sqlite
{
    public class SqliteConnection : IDbConnection
    {
        private IntPtr databasePointer;
        private bool isOpen;

        public SqliteConnection(string databaseName)
        {
            DatabaseName = databaseName;
        }

        public string DatabaseName { get; private set; }

        public void Open()
        {
            var result = Sqlite3.Open(DatabaseName, out databasePointer);
            if (result != SqliteResult.OK)
            {
                throw new SqliteException(string.Format("Could not open database file: {0}({1})", DatabaseName, result));
            }
            isOpen = true;
            Console.WriteLine("Connection opened");
        }

        public IDbCommand CreateCommand(DbCommandSpec commandSpec)
        {
            if (!isOpen)
            {
                throw new SqliteException("Cannot create commands from unopened database");
            }
            return new SqliteCommand(databasePointer, commandSpec);
        }

        public void BeginTransaction()
        {
            CreateCommand(new DbCommandSpec().SetCommandText("begin")).ExecuteNonQuery();
        }

        public void RollbackTransaction()
        {
            CreateCommand(new DbCommandSpec().SetCommandText("rollback")).ExecuteNonQuery();
        }

        public int GetLastInsertId()
        {
            var row = CreateCommand(new DbCommandSpec().SetCommandText("select last_insert_rowid()"))
                .ExecuteQuery();
            return (int)row.ToList()[0].ToList()[0].Value;
        }

        public void Dispose()
        {
            if (isOpen)
            {
                CreateCommand(new DbCommandSpec().SetCommandText("end")).ExecuteNonQuery();
                Sqlite3.Close(databasePointer);
                databasePointer = IntPtr.Zero;
                isOpen = false;
                Console.WriteLine("Connection closed");
            }
        }
    }
}