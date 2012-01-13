using Catnap.Maps;
using Catnap.Maps.Impl;
using Catnap.Tests.Core.Models;

namespace Catnap.Tests.Core
{
    public static class Bootstrapper
    {
        public static void ConfigureDomain()
        {
            Domain.Configure
            (
                Map.Entity<PersonGuid>()
                    .Id(x => x.Id, Access.Property, Generator.GuidComb)
                    .Property(x => x.FirstName)
                    .Property(x => x.LastName)
                    .Property(x => x.Active)
                    .Property(x => x.MemberSince),
                Map.Entity<ForumGuid>()
                    .Id(x => x.Id, Access.Property, Generator.GuidComb)
                    .List(x => x.Posts)
                    .Property(x => x.Name)
                    .Property(x => x.TimeOfDayLastUpdated),
                Map.Entity<PostGuid>()
                    .Id(x => x.Id, Access.Property, Generator.GuidComb)
                    .ParentColumn("ForumId")
                    .Property(x => x.Title)
                    .Property(x => x.Body)
                    .Property(x => x.DatePosted)
                    .BelongsTo(x => x.Poster, "PosterId"),

                Map.Entity<Person>()
                    .Property(x => x.FirstName)
                    .Property(x => x.LastName)
                    .Property(x => x.Active)
                    .Property(x => x.MemberSince),
                Map.Entity<Forum>()
                    .List(x => x.Posts)
                    .Property(x => x.Name)
                    .Property(x => x.TimeOfDayLastUpdated),
                Map.Entity<Post>()
                    .ParentColumn("ForumId")
                    .Property(x => x.Title)
                    .Property(x => x.Body)
                    .Property(x => x.DatePosted)
                    .BelongsTo(x => x.Poster, "PosterId")
            );
        }
    }
}