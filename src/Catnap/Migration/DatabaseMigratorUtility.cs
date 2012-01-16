using System.Linq;
using Catnap.Logging;

namespace Catnap.Migration
{
    public class DatabaseMigratorUtility
    {
        private readonly ISession session;
        private const string MIGRATIONS_TABLE_NAME = "db_migrations";

        public DatabaseMigratorUtility(ISession session)
        {
            this.session = session;
        }

        public void Migrate(params IDatabaseMigration[] migrations)
        {
            CreateMigrationsTableIfNotExists();
            foreach (var migration in migrations.Where(migration => !PreviouslyRun(migration)))
            {
                Log.Debug("Running migration '{0}'", migration.Name);
                migration.Action(session);
                RecordMigration(migration);
            }
        }

        private void RecordMigration(IDatabaseMigration migration)
        {
            var migrationsTableExistsCommand = new DbCommandSpec()
                .SetCommandText(string.Format("insert into {0} (Name) values ({1})", MIGRATIONS_TABLE_NAME,
                    session.FormatParameterName("name")))
                .AddParameter("name", migration.Name);
            session.ExecuteNonQuery(migrationsTableExistsCommand);
        }

        private bool PreviouslyRun(IDatabaseMigration migration)
        {
            var command = new DbCommandSpec()
                .SetCommandText(string.Format("select count(*) from {0} where Name = {1}",
                    MIGRATIONS_TABLE_NAME, session.FormatParameterName("name")))
                .AddParameter("name", migration.Name);
            var result = (long)session.ExecuteScalar(command);
            return result > 0;
        }

        private void CreateMigrationsTableIfNotExists()
        {
            var existsResult = session.GetTableMetaData(MIGRATIONS_TABLE_NAME);
        	if (existsResult.Count() != 0)
        	{
        		return;
        	}
        	var createMigrationsTable = new DbCommandSpec()
        		.SetCommandText(string.Format("create table {0} (Name varchar(200))", MIGRATIONS_TABLE_NAME));
            session.ExecuteNonQuery(createMigrationsTable);
        	Log.Debug("Migrations table created");
        }
    }
}