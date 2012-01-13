using System;
using System.Collections.Generic;
using System.Linq;
using Catnap.Database;
using Catnap.Migration;

namespace Catnap.IntegrationTests.Migrations
{
    public class CreateSchema : IDatabaseMigration
    {
        private readonly List<string> sqls = new List<string>
        {
            @"CREATE TABLE Forum (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, Name VARCHAR, TimeOfDayLastUpdated NUMERIC)",
            @"CREATE TABLE Post (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, Title VARCHAR, Body VARCHAR, DatePosted INTEGER, PosterId INTEGER, ForumId INTEGER NOT NULL)",
            @"CREATE TABLE Person (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, FirstName VARCHAR, LastName VARCHAR, Active INTEGER, MemberSince INTEGER)",

            @"CREATE TABLE ForumGuid (Id TEXT PRIMARY KEY NOT NULL, Name VARCHAR, TimeOfDayLastUpdated NUMERIC)",
            @"CREATE TABLE PostGuid (Id TEXT PRIMARY KEY NOT NULL, Title VARCHAR, Body VARCHAR, DatePosted INTEGER, PosterId TEXT, ForumId TEXT NOT NULL)",
            @"CREATE TABLE PersonGuid (Id TEXT PRIMARY KEY NOT NULL, FirstName VARCHAR, LastName VARCHAR, Active INTEGER, MemberSince INTEGER)",
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