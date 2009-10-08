namespace Catnap
{
    public static class SessionFactory
    {
        private static string connString;

        public static void Initialize(string connectionString)
        {
            connString = connectionString;
        }

        public static Session New()
        {
            return new Session(connString);
        }
    }
}