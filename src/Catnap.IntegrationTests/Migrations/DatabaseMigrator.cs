using Catnap.Database.Sqlite;
using Catnap.Migration;

namespace Catnap.IntegrationTests.Migrations
{
    public static class DatabaseMigrator
    {
        private static readonly DatabaseMigratorUtility migratorUtility = new DatabaseMigratorUtility(DbAdapter.Sqlite);

        public static void Execute()
        {
            migratorUtility.Migrate
            (
                new CreateSchema()
            );
        }
    }
}