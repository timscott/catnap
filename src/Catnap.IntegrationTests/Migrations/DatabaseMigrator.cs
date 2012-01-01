using Catnap.Migration;

namespace Catnap.IntegrationTests.Migrations
{
    public static class DatabaseMigrator
    {
        public static void Execute(ISession session)
        {
            new DatabaseMigratorUtility(session).Migrate
            (
                new CreateSchema()
            );
        }
    }
}