using Catnap.Adapters;
using Catnap.Common.Logging;
using Catnap.IntegrationTests.Migrations;
using Catnap.Maps;
using Catnap.Maps.Impl;
using Catnap.Tests.Core.Models;

namespace Catnap.IntegrationTests
{
    public class Initializer
    {
        public void InitializeWithGuidIds()
        {
            Log.Level = LogLevel.Debug;
            SessionFactory.Initialize("Data source=:memory:", new SqliteAdapter());
            Domain.Configure
                (
                    Map.Entity<Person>()
                        .Id(x => x.Id, Access.Property, Generator.GuidComb)
                        .Property(x => x.FirstName)
                        .Property(x => x.LastName),
                    Map.Entity<Forum>()
                        .Id(x => x.Id, Access.Property, Generator.GuidComb)
                        .List(x => x.Posts)
                        .Property(x => x.Name)
                        .Property(x => x.TimeOfDayLastUpdated),
                    Map.Entity<Post>()
                        .Id(x => x.Id, Access.Property, Generator.GuidComb)
                        .ParentColumn("ForumId")
                        .Property(x => x.Title)
                        .Property(x => x.Body)
                        .Property(x => x.DatePosted)
                        .BelongsTo(x => x.Poster, "PosterId")
                );
            UnitOfWork.Start(); //NOTE: Normally unit-of work-would be more fine grained; however the in-memory database is created blank with each connection
            DatabaseMigrator.Execute(new CreateSchemaTextIds());
        }

        public void InitializeDefault()
        {
            Log.Level = LogLevel.Debug;
            SessionFactory.Initialize("Data source=:memory:", new SqliteAdapter());
            Domain.Configure
                (
                    Map.Entity<PersonInt>()
                        .Property(x => x.FirstName)
                        .Property(x => x.LastName),
                    Map.Entity<ForumInt>()
                        .List(x => x.Posts)
                        .Property(x => x.Name)
                        .Property(x => x.TimeOfDayLastUpdated),
                    Map.Entity<PostInt>()
                        .ParentColumn("ForumId")
                        .Property(x => x.Title)
                        .Property(x => x.Body)
                        .Property(x => x.DatePosted)
                        .BelongsTo(x => x.Poster, "PosterId")
                );
            UnitOfWork.Start(); //NOTE: Normally unit-of work-would be more fine grained; however the in-memory database is created blank with each connection
            DatabaseMigrator.Execute(new CreateSchema());
        }
    }
}