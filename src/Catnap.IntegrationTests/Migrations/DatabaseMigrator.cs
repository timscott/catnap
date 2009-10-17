using Catnap.Migration;
using Catnap.Sqlite;

namespace Catnap.IntegrationTests.Migrations
{
    public static class DatabaseMigrator
    {
        private static readonly DatabaseMigratorUtility migratorUtility =
            new DatabaseMigratorUtility(new SqliteMetadataCommandFactory());

        public static void Execute()
        {
            migratorUtility.Migrate
                (
                new CreateSchema()
                );
        }
    }
}