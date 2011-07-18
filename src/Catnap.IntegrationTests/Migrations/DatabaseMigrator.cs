using Catnap.Adapters;
using Catnap.Adapters.Sqlite;
using Catnap.Migration;

namespace Catnap.IntegrationTests.Migrations
{
    public static class DatabaseMigrator
    {
        private static readonly DatabaseMigratorUtility migratorUtility = new DatabaseMigratorUtility(new SqliteAdapter());

        public static void Execute()
        {
            migratorUtility.Migrate
            (
                new CreateSchema()
            );
        }
    }
}