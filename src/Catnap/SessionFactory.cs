using System;
using Catnap.Database;

namespace Catnap
{
    public class SessionFactory : ISessionFactory
    {
        private static ISessionFactory current;

        private readonly IDbAdapter dbAdapter;
        private readonly string connectionString;

        public SessionFactory(string connectionString, IDbAdapter dbAdapter)
        {
            this.connectionString = connectionString;
            this.dbAdapter = dbAdapter;
        }

        public static ISessionFactory Current
        {
            get
            {
                if (current == null)
                {
                    throw new InvalidOperationException("SessionFactory not initialized.");
                }
                return current;
            }
        }

        internal static void Initialize(string connectionString, IDbAdapter dbAdapter)
        {
            current = new SessionFactory(connectionString, dbAdapter);
        }

        public ISession New()
        {
            return new Session(connectionString, dbAdapter);
        }

        //TODO: This does not belong here
        public string FormatParameterName(string name)
        {
            const string defaultSqlParameterPrefix = "@";
            return dbAdapter == null
                ? name.StartsWith(defaultSqlParameterPrefix)
                    ? name
                    : defaultSqlParameterPrefix + name
                : dbAdapter.FormatParameterName(name);
        }
    }
}