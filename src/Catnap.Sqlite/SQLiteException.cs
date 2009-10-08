using System;

namespace Catnap.Sqlite
{
    public class SqliteException : Exception
    {
        public SqliteException(string message, params object[] args) : base(GetMessage(message, args)) { }

        private static string GetMessage(string message, object[] args)
        {
            return args == null ? null : string.Format(message, args);
        }
    }
}