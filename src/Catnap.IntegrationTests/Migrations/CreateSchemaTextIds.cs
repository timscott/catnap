using System;
using System.Collections.Generic;
using System.Linq;
using Catnap.Database;
using Catnap.Migration;

namespace Catnap.IntegrationTests.Migrations
{
    public class CreateSchemaTextIds : IDatabaseMigration
    {
        private readonly List<string> sqls = new List<string>
        {
            @"CREATE TABLE Forum (Id TEXT PRIMARY KEY NOT NULL, Name VARCHAR, TimeOfDayLastUpdated NUMERIC)",
            @"CREATE TABLE Post (Id TEXT PRIMARY KEY NOT NULL, Title VARCHAR, Body VARCHAR, DatePosted INTEGER, PosterId TEXT, ForumId TEXT NOT NULL)",
            @"CREATE TABLE Person (Id TEXT PRIMARY KEY NOT NULL, FirstName VARCHAR, LastName VARCHAR)",
        };

        public string Name
        {
            get { return "create_schema"; }
        }

        public Action Action
        {
            get { return () => sqls.Select(x => new DbCommandSpec().SetCommandText(x)).ToList().ForEach(Execute); }
        }

        private void Execute(DbCommandSpec command)
        {
            UnitOfWork.Current.Session.ExecuteNonQuery(command);
        }
    }
}