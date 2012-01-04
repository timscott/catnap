using System;
using Catnap.Mapping;
using Catnap.Tests.Core.Models;

namespace Catnap.Tests.Core
{
    public static class DomainMapping
    {
        public static Action<IDomainMappable> Get()
        {
            return d =>
            {
                //d.IdConvention().Column(x => x.EntityType.Name + "Id").Access(Access.Property).Generator(Generator.GuidComb);
                //d.ListParentIdColumnNameConvention(x => x.ParentType.Name + "_id");
                //d.BelongsToColumnNameConvention(x => x.PropertyName + "_id");

                d.Entity<PersonGuid>(e => {
                    e.Id(x => x.Id).Access(Access.Property).Generator(Generator.GuidComb);
                    e.Property(x => x.FirstName);
                    e.Property(x => x.LastName);
                    e.Property(x => x.Active);
                    e.Property(x => x.MemberSince);
                });
                d.Entity<ForumGuid>(e => {
                    e.Id(x => x.Id).Access(Access.Property).Generator(Generator.GuidComb);
                    e.List(x => x.Posts).ParentIdColumn("ForumId");
                    e.Property(x => x.Name);
                    e.Property(x => x.TimeOfDayLastUpdated);
                });
                d.Entity<PostGuid>(e => {
                    e.Id(x => x.Id).Access(Access.Property).Generator(Generator.GuidComb);
                    e.Property(x => x.Title);
                    e.Property(x => x.Body);
                    e.Property(x => x.DatePosted);
                    e.BelongsTo(x => x.Poster);
                });

                d.Entity<Person>(e => {
                    e.Property(x => x.FirstName);
                    e.Property(x => x.LastName);
                    e.Property(x => x.Active);
                    e.Property(x => x.MemberSince);
                });
                d.Entity<Forum>(e => {
                    e.List(x => x.Posts);
                    e.Property(x => x.Name);
                    e.Property(x => x.TimeOfDayLastUpdated);
                });
                d.Entity<Post>(e => {
                    e.Property(x => x.Title);
                    e.Property(x => x.Body);
                    e.Property(x => x.DatePosted);
                    e.BelongsTo(x => x.Poster);
                });
            };
        }
    }
}