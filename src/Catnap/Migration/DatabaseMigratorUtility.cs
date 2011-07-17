using System.Linq;
using Catnap.Common.Database;
using Catnap.Common.Logging;

namespace Catnap.Migration
{
    public class DatabaseMigratorUtility
    {
        private const string MIGRATIONS_TABLE_NAME = "db_migrations";
        private readonly IMetadataCommandFactory metadataCommandFactory;

        public DatabaseMigratorUtility(IMetadataCommandFactory metadataCommandFactory)
        {
            this.metadataCommandFactory = metadataCommandFactory;
        }

        public void Migrate(params IDatabaseMigration[] migrations)
        {
            CreateMigrationsTableIfNotExists();
            foreach (var migration in migrations)
            {
                if (!PreviouslyRun(migration))
                {
                    Log.Debug("Running migration '{0}'", migration.Name);
                    migration.Action();
                    RecordMigration(migration);
                }
            }
        }

        private void RecordMigration(IDatabaseMigration migration)
        {
            var migrationsTableExistsCommand = new DbCommandSpec()
                .SetCommandText(string.Format("insert into {0} (Name) values (@name)", MIGRATIONS_TABLE_NAME))
                .AddParameter("@name", migration.Name);
            UnitOfWork.Current.Session.ExecuteNonQuery(migrationsTableExistsCommand);
        }

        private bool PreviouslyRun(IDatabaseMigration migration)
        {
            var command = new DbCommandSpec()
                .SetCommandText(string.Format(@"select count(*) from {0} where Name = @name", MIGRATIONS_TABLE_NAME))
                .AddParameter("name", migration.Name);
            var result = (long)UnitOfWork.Current.Session.ExecuteScalar(command);
            return result > 0;
        }

        private void CreateMigrationsTableIfNotExists()
        {
            var existsResult = UnitOfWork.Current.Session.ExecuteQuery(
                metadataCommandFactory.GetGetTableMetadataCommand(MIGRATIONS_TABLE_NAME));
        	if (existsResult.Count() != 0)
        	{
        		return;
        	}
        	var createMigrationsTable = new DbCommandSpec()
        		.SetCommandText(string.Format("create table {0} (Name varchar(200))", MIGRATIONS_TABLE_NAME));
        	UnitOfWork.Current.Session.ExecuteNonQuery(createMigrationsTable);
        	Log.Debug("Migrations table created");
        }
    }
}