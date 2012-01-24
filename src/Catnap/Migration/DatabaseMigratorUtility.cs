using System;
using System.Data;
using System.Linq;
using Catnap.Logging;

namespace Catnap.Migration
{
    public class DatabaseMigratorUtility
    {
        private readonly ISession session;
        private const string MIGRATIONS_TABLE_NAME = "db_migrations";
        private const string MIGRATION_NAME_COLUMN_NAME = "Name";

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
                .SetCommandText(string.Format("insert into {0} ({1}) values ({2})", 
                    MIGRATIONS_TABLE_NAME,
                    session.DbAdapter.Quote(MIGRATION_NAME_COLUMN_NAME),
                    session.DbAdapter.FormatParameterName("name")))
                .AddParameter("name", migration.Name);
            session.ExecuteNonQuery(migrationsTableExistsCommand);
        }

        private bool PreviouslyRun(IDatabaseMigration migration)
        {
            var command = new DbCommandSpec()
                .SetCommandText(string.Format("select count(*) from {0} where {1} = {2}",
                    MIGRATIONS_TABLE_NAME, 
                    session.DbAdapter.Quote(MIGRATION_NAME_COLUMN_NAME),
                    session.DbAdapter.FormatParameterName("name")))
                .AddParameter("name", migration.Name);
            var result = session.ExecuteScalar(command);
            return Convert.ToInt64(result) > 0;
        }

        private void CreateMigrationsTableIfNotExists()
        {
            var tableExists = session.TableExists(MIGRATIONS_TABLE_NAME);
            if (tableExists)
            {
                return;
            }
            var createMigrationsTable = new DbCommandSpec()
                .SetCommandText(string.Format("create table {0} ({1} {2})", 
                    MIGRATIONS_TABLE_NAME,
                    session.DbAdapter.Quote(MIGRATION_NAME_COLUMN_NAME),
                    session.DbAdapter.GetGeneralStringType()));
            session.ExecuteNonQuery(createMigrationsTable);
            Log.Debug("Migrations table created");
        }
    }
}