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
                d => d.Entity<PersonGuid>(
                    e => e.Id(x => x.Id).Access(Access.Property).Generator(Generator.GuidComb),
                    e => e.Property(x => x.FirstName),
                    e => e.Property(x => x.LastName),
                    e => e.Property(x => x.Active),
                    e => e.Property(x => x.MemberSince)),
                d => d.Entity<ForumGuid>(
                    e => e.Id(x => x.Id).Access(Access.Property).Generator(Generator.GuidComb),
                    e => e.List(x => x.Posts),
                    e => e.Property(x => x.Name),
                    e => e.Property(x => x.TimeOfDayLastUpdated)),
                d => d.Entity<PostGuid>(
                    e => e.Id(x => x.Id).Access(Access.Property).Generator(Generator.GuidComb),
                    e => e.Property(x => x.Title),
                    e => e.Property(x => x.Body),
                    e => e.Property(x => x.DatePosted),
                    e => e.BelongsTo(x => x.Poster).ColumnName("PosterId"))
                    .ParentColumn("ForumId"),

                d => d.Entity<Person>(
                    e => e.Property(x => x.FirstName),
                    e => e.Property(x => x.LastName),
                    e => e.Property(x => x.Active),
                    e => e.Property(x => x.MemberSince)),
                d => d.Entity<Forum>(
                    e => e.List(x => x.Posts),
                    e => e.Property(x => x.Name),
                    e => e.Property(x => x.TimeOfDayLastUpdated)),
                d => d.Entity<Post>(
                    e => e.Property(x => x.Title),
                    e => e.Property(x => x.Body),
                    e => e.Property(x => x.DatePosted),
                    e => e.BelongsTo(x => x.Poster).ColumnName("PosterId"))
                    .ParentColumn("ForumId")
            );
        }
    }
}