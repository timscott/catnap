using System;
using System.Data.SqlServerCe;
using System.IO;
using Catnap.Database;
using Catnap.IntegrationTests.Migrations;
using Catnap.Logging;
using Catnap.Migration;
using Catnap.Tests.Core;

namespace Catnap.IntegrationTests
{
    public static class Bootstrapper
    {
        public static ISessionFactory BootstrapSqlite()
        {
            return Bootstrap(new CreateSchema_Sqlite(), DbAdapter.Sqlite, null);
        }

        public static ISessionFactory BootstrapSqlServerCe()
        {
            return Bootstrap(new CreateSchema_SqlServerCe(), DbAdapter.SqlServerCe, cs =>
            {
                var engine = new SqlCeEngine(cs);
                engine.CreateDatabase();
            });
        }
        
        public static ISessionFactory BootstrapMySql()
        {
        	var sessionFactory = Fluently.Configure
        	.ConnectionString(System.Configuration.ConfigurationManager.ConnectionStrings["MySql"].ConnectionString)
		    .DatabaseAdapter(DbAdapter.MySql)
			.Domain(Catnap.Tests.Core.DomainMapping.Get())
		    .Build();

			UnitOfWork.Initialize(sessionFactory);
			using (var s = sessionFactory.Create())
            {
                s.Open();
                if(!s.TableExists("forum"))
                {
                	new DatabaseMigratorUtility(s).Migrate(new CreateSchema_MySql());
                }
                else
                {
	                s.ExecuteNonQuery(new DbCommandSpec().SetCommandText("DELETE FROM forum"));
	                s.ExecuteNonQuery(new DbCommandSpec().SetCommandText("DELETE FROM forumguid"));
	                s.ExecuteNonQuery(new DbCommandSpec().SetCommandText("DELETE FROM person"));
	                s.ExecuteNonQuery(new DbCommandSpec().SetCommandText("DELETE FROM personguid"));
	                s.ExecuteNonQuery(new DbCommandSpec().SetCommandText("DELETE FROM post"));
	                s.ExecuteNonQuery(new DbCommandSpec().SetCommandText("DELETE FROM postguid"));
                }
			}
			
        	return sessionFactory;        	
        }

        private static ISessionFactory Bootstrap(IDatabaseMigration createSchema, IDbAdapter dbAdapter, Action<string> createDatabaseFunc)
        {
            Log.Level = LogLevel.Off;
            const string dbFileName = "main.db";
            const string connectionString = "Data source=" + dbFileName;
            File.Delete(dbFileName);
            if (createDatabaseFunc != null)
            {
                createDatabaseFunc(connectionString);
            }
            var sessionFactory = Fluently.Configure
                .ConnectionString(connectionString)
                .DatabaseAdapter(dbAdapter)
                .Domain(DomainMapping.Get())
                .Build();
            UnitOfWork.Initialize(sessionFactory);
            using (var s = sessionFactory.Create())
            {
                s.Open();
                new DatabaseMigratorUtility(s).Migrate(createSchema);
            }
            return sessionFactory;
        }
    }
}